import { DatePicker, Divider, Form, Input, Modal, Select, SelectProps } from 'antd';
import React, { useEffect, useRef, useState } from 'react';
// import { IModalProp } from '../../types/IModalProp';
import './ModalCourse.css';
import dayjs from 'dayjs';
import { User } from '../../hooks/useAuth';
import { IDataGetCourse } from '../../types/IDataGetCourse';
import { getReq, patchReq } from '../../services/api';
import utc from 'dayjs/plugin/utc';

const ModalEditCourse: React.FC<any> = ({modalState, onCancelClick, course, onRefresh, returnMessage}) => {
    const didFetch = useRef(false);
    dayjs.extend(utc);
    const [form] = Form.useForm();
    const [loading, setLoading] = React.useState<boolean>(true);
    const [lecturer, setLecturer] = useState<User[]>();
    const [courseData, setCourseData] = useState<IDataGetCourse>();
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

    // Validate and Call Api
    const handleOk = async() => {
        form
        .validateFields({recursive: true})
        .then(async values => {
            console.log('values: ',values);
            
            // const endDateString = values.time[1].toISOString();
            console.log("values startDate:  ", values.time[0]);
            console.log("course startDate:  ", courseData?.startDate.slice(0,10));
            
            var courseDTO: any = {};
            if (values.courseName !== courseData?.courseName){
                courseDTO.courseName = values.courseName;
            }
            if (values.lecturerId !== courseData?.lecturer.lecturerId){
                courseDTO.lecturerId = values.lecturerId;
            }
            // startDate
            if (values.time[0].format('YYYY-MM-DD') !== courseData?.startDate.slice(0,10)){
                const startDateString = values.time[0].format('YYYY-MM-DD') + 'T00:00:00Z'; // Đặt giờ về 00:00:00 và thêm Z để chỉ định UTC 
                console.log("startDateString:  ", startDateString);
                courseDTO.startDate = startDateString;
            }
            // endDate
            if (values.time[1].format('YYYY-MM-DD')  !== courseData?.endDate.slice(0,10)){
                const endDateString = values.time[1].format('YYYY-MM-DD') + 'T00:00:00Z';
                courseDTO.endDate = endDateString;
            }

            // Lấy danh sách shiftId từ courseData
            const oldShifts = courseData?.shifts || [];
            const newShiftsString = values.shifts || [];

            // Tạo key: "weekDay - shiftOfDay"
            const makeKey = (shift: any) => `${shift.weekDay} - ${shift.shiftOfDay}`;

            // So sánh shifts
            const isShiftChanged =
                oldShifts.length !== newShiftsString.length ||
                !oldShifts.every((oldShift: any) => newShiftsString.includes(makeKey(oldShift)));

            console.log('shift changed? :', isShiftChanged);

            if (isShiftChanged) {
                // Cập nhật courseDTO với shiftId từ courseData
                courseDTO.shifts = values.shifts.map((shift: string) => {
                    const shiftDay = shift.split(' - ');
                    const shiftString = `${shiftDay[0]} - ${shiftDay[1]}`;

                    // Kiểm tra nếu shiftString có trong oldShifts
                    const oldShift = oldShifts.find(old => `${old.weekDay} - ${old.shiftOfDay}` === shiftString);

                    if (oldShift) {
                        // Nếu là shift cũ, lấy shiftId
                        return {
                            shiftCode: oldShift.shiftOfDay,
                            weekDay: oldShift.weekDay,
                            maxQuantity: 25,
                            shiftId: oldShift.shiftId // Lấy shiftId từ courseData
                        };
                    } else {
                        // Nếu là shift mới, không cần shiftId
                        return {
                            shiftCode: shiftDay[1], 
                            weekDay: shiftDay[0],    
                            maxQuantity: 25
                        };
                    }
                });
            }
            console.log(courseDTO);
            if (Object.keys(courseDTO).length === 0 ){
                returnMessage('Nothing to change', false);
                onCancelClick();
            }
            else {

                try {
                    const result = await patchReq<IDataGetCourse>('/Courses/' + course?.courseId, courseDTO);
                    console.log('Course patched:', result);
                    onRefresh();
                    returnMessage('Edit course successfully!', true);
                    onCancelClick();
                    // 
                }
                catch(error: any){
                    console.log("Error while patching: ", error);
                    returnMessage(error.response.data, false);
                }
            }
        })
        .catch(errorInfo => {
            // lỗi validate:
            console.log("Validation Failed:", errorInfo);
            returnMessage(errorInfo.errorFields[0].errors[0], false);
        });
    };

    const handleCancel = () => {
        console.log(`modal state: ${modalState}`);
        onCancelClick();
    };

    const { RangePicker } = DatePicker;

    // FETCH------------
    const loadData = async () => {
        setLoading(true);
        try {
            const result = await getReq<User[]>('/User/all-lecturer', {});
            const res = await getReq<IDataGetCourse>('/Courses/' + course.courseId, {});
            console.log("all lecturers:  ", result);
            console.log("data of course: ", res);
            setLecturer(result);
            setCourseData(res)
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
            console.log('courseData: ', course);
            didFetch.current = true;
        }
    },[course]);

    return (
        <>
            <Modal className='modal' title="Edit course" open={modalState} onOk={handleOk} onCancel={handleCancel} loading={loading}>
                <Divider/>
                <Form className='form'
                    form={form}
                    labelCol={{ span: 6 }}
                    wrapperCol={{ span: 16 }}
                    layout="horizontal"
                >
                    <Form.Item initialValue={course?.courseName} name="courseName" label="Course name" rules={[{ required: true, message: 'Course name is required' }]}>
                        <Input defaultValue={course?.courseName} />
                    </Form.Item>
                    <Form.Item initialValue={courseData?.lecturer.lecturerId} name="lecturerId" label="Lecturer" rules={[{ required: true, message: 'LecturerId is required' }]}>
                        <Select 
                            defaultValue={courseData?.lecturer.lecturerId} 
                            options={lecturer?.map((lec) => ({
                                label: lec.firstName + ' ' + lec.lastName,
                                value: lec.id
                            }))}
                        />
                    </Form.Item>
                    <Form.Item initialValue={courseData?.shifts.map((shift) => `${shift.weekDay} - ${shift.shiftOfDay}`)} name="shifts" label="Shifts" rules={[{ required: true, message: 'Shift is required and more than or equal to 2', min: 2, type: 'array' }]}>
                        <Select
                            mode="multiple"
                            allowClear
                            style={{ width: '100%' }}
                            placeholder="Please select"
                            defaultValue={courseData?.shifts.map((shift) => `${shift.weekDay} - ${shift.shiftOfDay}`)}
                            onChange={handleChange}
                            options={options}
                        />
                    </Form.Item>
                    <Form.Item initialValue={[dayjs(course?.startDate, 'YYYY/MM/DD'), dayjs(course?.endDate, 'YYYY/MM/DD')]} name="time" label="Time" rules={[{ required: true, message: 'Range time of the course is required' }]}>
                        <RangePicker format={'DD/MM/YYYY'} />
                    </Form.Item>
                </Form>
                <Divider/>
            </Modal>
        </>
    );
}
export default ModalEditCourse;