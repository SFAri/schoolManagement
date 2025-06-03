import { DatePicker, Divider, Form, Input, message, Modal, Select, SelectProps } from 'antd';
import React, { useEffect, useRef, useState } from 'react';
import { IModalProp } from '../../types/IModalProp';
import './ModalCourse.css';
import { User } from '../../hooks/useAuth';
import { getReq, postReq } from '../../services/api';
import { IDataGetCourse } from '../../types/IDataGetCourse';

const ModalCourse: React.FC<IModalProp> = ({modalState, onCancelClick, onRefresh, returnMessage}) => {
    const [form] = Form.useForm();
    const didFetch = useRef(false);
    const [loading, setLoading] = React.useState<boolean>(true);
    // Option here is mixed of WeekDay + ShiftCode (VD: "Monday - Morning", "Tuesday - Afternoon" ,...)
    const options: SelectProps['options'] = [
        {label: 'Monday - Morning', value:'Monday - Morning'},
        {label: 'Tuesday - Morning', value:'Tuesday - Morning'},
        {label: 'Wednesday - Morning', value:'Wednesday - Morning'},
        {label: 'Thursday - Morning', value:'Thursday - Morning'},
        {label: 'Friday - Morning', value:'Friday - Morning'},
        {label: 'Saturday - Morning', value:'Saturday - Morning'},

        {label: 'Monday - Afternoon', value:'Monday - Afternoon'},
        {label: 'Tuesday - Afternoon', value:'Tuesday - Afternoon'},
        {label: 'Wednesday - Afternoon', value:'Wednesday - Afternoon'},
        {label: 'Thursday - Afternoon', value:'Thursday - Afternoon'},
        {label: 'Friday - Afternoon', value:'Friday - Afternoon'},
        {label: 'Saturday - Afternoon', value:'Saturday - Afternoon'},
    ];

    const [lecturer, setLecturer] = useState<User[]>();

    const handleChange = (value: string[]) => {
    console.log(`selected ${value}`);
    };

    // Validate and Call Api
    const handleOk = async () => {
        form
        .validateFields({recursive: true})
        .then(async values => {
            console.log(values);
            // Convert to Date object
            const convert0 = new Date(values.time[0].$d);
            const convert1 = new Date(values.time[1].$d);
            // Convert to ISO string
            const startDateString = convert0.toISOString();
            const endDateString = convert1.toISOString();
            const shifts = values.shifts.map((shift: any) => {
                const shiftDay = shift.split(' - ');
                return {
                    shiftCode: shiftDay[1],
                    weekDay: shiftDay[0],
                    maxQuantity: 25
                }
            })
            // console.log("shifts:", shifts)
            const courseDTO = {
                courseName: values.courseName.toString(),
                lecturerId: values.lecturerId.toString(),
                startDate: startDateString,
                endDate: endDateString,
                shifts: shifts
            }

            console.log(courseDTO);
            try {
                const result = await postReq<IDataGetCourse>('/Courses', courseDTO);
                if (result){
                    returnMessage( 'Add new course successfully!', true);
                }
                onRefresh();
                console.log('Course created:', result);
                onCancelClick();
            }
            catch(error: any){
                console.log("Error while posting: ", error.response?.data)
                returnMessage(error.response?.data, false);
            }

        })
        .catch(errorInfo => {
            // ❌ lỗi validate:
            console.log("Validation Failed:", errorInfo);
            returnMessage(errorInfo.errorFields[0].errors[0], false);
        });
    };

    const handleCancel = () => {
        onCancelClick();
    };

    // const {data, loading, error} = useFetch<User[]>('/User/all-lecturer', {}, "GET");

    const { RangePicker } = DatePicker;

    const loadData = async () => {
            setLoading(true);
            try {
                const result = await getReq<User[]>('/User/all-lecturer', {});
                setLecturer(result);
            } catch (error: any) {
                if (error.name === 'CanceledError') {
                    console.log('Request canceled: ', error.message);
                } else {
                    message.error('Lỗi khi load dữ liệu 😢');
                    console.error(error);
                }
            } finally {
                setLoading(false);
            }
        }

    useEffect(() => {
        if (!didFetch.current) {
            loadData();
            didFetch.current = true;
        }
    },[]);

    return (
        <>
            <Modal className='modal' title="Create new course" open={modalState} onOk={handleOk} onCancel={handleCancel} loading={loading}>
                <Divider/>
                <Form className='form'
                    form={form}
                    labelCol={{ span: 6 }}
                    wrapperCol={{ span: 16 }}
                    onFinish={handleOk}
                    layout="horizontal"
                >
                    <Form.Item  label="Course name" name="courseName" rules={[{ required: true, message: 'Course name is required' }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item label="Lecturer" name="lecturerId" rules={[{ required: true, message: 'LecturerId is required' }]}>
                        <Select>
                            {lecturer?.map((lec)=> {
                                return (
                                    <Select.Option key={lec.id} value={lec.id}>{lec.firstName + ' ' + lec.lastName}</Select.Option>
                                );
                            })}
                        </Select>
                    </Form.Item>
                    <Form.Item name="shifts" label="Shifts" rules={[{ required: true, message: 'Shift is required and more than or equal to 2', min: 2, type: 'array' }]}>
                        <Select
                            mode="multiple"
                            allowClear
                            style={{ width: '100%' }}
                            placeholder="Please select shift"
                            onChange={handleChange}
                            options={options}
                        />
                    </Form.Item>
                    <Form.Item name="time" label="Time" rules={[{ required: true, message: 'Range time of the course is required' }]}>
                        <RangePicker />
                    </Form.Item>
                </Form>
                <Divider/>
            </Modal>
        </>
    );
}
export default ModalCourse;