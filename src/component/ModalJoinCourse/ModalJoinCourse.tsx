import { Divider, Form, Modal, Select, SelectProps } from 'antd';
import React, { useEffect, useRef, useState } from 'react';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import { getReq, postReq } from '../../services/api';
import { IDataGetCourse } from '../../types/IDataGetCourse';
import { IRowShift } from '../../types/IRowShift';

const ModalJoinCourse: React.FC<any> = ({modalState, onCancelClick, isEdit=false, onRefresh, returnMessage}) => {
    const didFetch = useRef(false);
    const [courses, setCourses] = useState<IDataGetCourse[]>([]);
    dayjs.extend(utc);
    const [form] = Form.useForm();
    const [loading, setLoading] = React.useState<boolean>(true);
    const [selectedCourse, setSelectedCourse] = useState(null);
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

    const handleChange = (value: string[]) => {
        console.log(`selected ${value}`);
    };

    const handleChangeCourse = (value: any) => {
        form.setFieldsValue({ shifts: [] });
        setSelectedCourse(value);
    };

    const getAvailableShifts = () => {
        if (selectedCourse) {
            const courseData = courses.find((course: IDataGetCourse) => course.courseId === selectedCourse);
            if (!courseData || !courseData.shifts) {
              return options; // Or [], depending on your desired behavior
            }
        
            const existingShiftValues = courseData.shifts.map(
              (s: IRowShift) => { return {label: `${s.weekDay} - ${s.shiftOfDay}`, value : s.shiftId} } // Create a string for comparison
            );
        
            return existingShiftValues;
        }
        return options;
    }

    // Validate and Call Api
    const handleOk = () => {
        form
        .validateFields({recursive: true})
        .then(async values => {
            console.log(values);
            const joinDTO = {
                shiftsId : values.shifts,
                courseId : values.courseId
            }
            console.log(joinDTO );
            try {
                const result = await postReq<any>('/Courses/join', joinDTO);
                if (result){
                    returnMessage(result, true);
                }
                onRefresh();
                form.resetFields();
                onCancelClick();
            }
            catch(error: any){
                console.log("Error while joining course: ", error)
                returnMessage(error.response?.data, false);
            }

        })
        .catch(errorInfo => {
            // // ❌ lỗi validate:
            console.log("Validation Failed:", errorInfo);
            returnMessage(errorInfo.errorFields[0].errors[0], false);
        });
    };

    const handleCancel = () => {
        onCancelClick();
    };

    // FETCH------------
    const loadData = async () => {
        setLoading(true);
        try {
            const res = await getReq<any>('/Courses/not-joined', {});
            console.log("courses: ", res);
            setCourses(res.data);
        } catch (error: any) {
            if (error.name === 'CanceledError') {
                console.log('Request canceled: ', error.message);
            } else {
                console.error(error);
            }
        } finally {
            setLoading(false);
        }
    }
    
    useEffect(() => {
        if (!didFetch.current) {
            loadData();
            console.log('courseData: ', courses);
            didFetch.current = true;
        }
    },[courses]);

    return (
        <>
            <Modal className='modal' title="Join new course" open={modalState} onOk={handleOk} onCancel={handleCancel} loading={loading}>
                <Divider/>
                <Form className='form'
                    form={form}
                    labelCol={{ span: 6 }}
                    wrapperCol={{ span: 16 }}
                    layout="horizontal"
                >
                    <Form.Item label="Course:" name="courseId" rules={[{required: true}]}>
                        <Select placeholder='Select new course' disabled={isEdit} onChange={handleChangeCourse}
                            options={courses?.map((c) => ({
                                label: c.courseName,
                                value: c.courseId
                            }))}
                        />
                    </Form.Item>
                    <Form.Item label="Shift:" name="shifts" rules={[{required: true, min: 2, message: 'Please select at least 2 shifts', type: 'array'}]}>
                        <Select
                            disabled={selectedCourse ? false : true}
                            mode="multiple"
                            allowClear
                            maxCount={2}
                            style={{ width: '100%' }}
                            placeholder="Please select"
                            onChange={handleChange}
                        >
                            {getAvailableShifts()?.map(shift => (
                                <Select.Option key={shift.value} value={shift.value}>{shift.label}</Select.Option>
                            ))}
                        </Select>
                    </Form.Item>
                </Form>
                <Divider/>
            </Modal>
        </>
    );
}
export default ModalJoinCourse;