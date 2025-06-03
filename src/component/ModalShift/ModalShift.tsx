import { Divider, Form, InputNumber, Modal, Select} from 'antd';
import React, { useState } from 'react';
import './ModalShift.css';
import { EShiftCode } from '../../types/EShiftCode';
import { EWeekDay } from '../../types/EWeekDay';
import { postReq } from '../../services/api';
import { IRowShift } from '../../types/IRowShift';

const ModalShift: React.FC<any> = ({ courseData, modalState, onCancelClick, onRefresh, returnMessage}) => {
    // Validate and Call Api
    const [form] = Form.useForm();
    const [loading, setLoading] = useState<boolean>(false);
    // const [weekDay, setWeekDay] = useState<any>();
    // const [shiftOfDay, setShiftOfDay] = useState<any>();
    const allWeekDays = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    const allShifts = ["Morning", "Afternoon"];

    // Tình trạng để lưu ngày được chọn
    const [selectedWeekDay, setSelectedWeekDay] = useState(null);

    // Lọc các ngày chưa có trong listShifts
    const availableWeekDays = allWeekDays.filter(day => {
        const shiftsForDay = courseData.shifts.filter((shift: IRowShift) => shift.weekDay === day); //Lọc ra những shifts trong ngày được chọn trùng với ngày trong courseData.shifts
        const shiftTypes = shiftsForDay.map((shift: IRowShift) => shift.shiftOfDay); // Lọc ra list shiftCode của day (nếu độ dài của list này == allShifts thì nghĩa là các shifts của ngày này đã đc chọn hết)

        console.log("ShiftsForDays: ", shiftsForDay);
        console.log("ShiftsTypes: ", shiftTypes);
        
        // Kiểm tra điều kiện: chưa chọn hoặc chưa đủ ca
        if (!selectedWeekDay) {
            return true; // Nếu chưa có ngày nào được chọn, hiển thị tất cả
        }
        
        return shiftTypes.length < allShifts.length || day === selectedWeekDay;
    });
    // Lọc ca dựa trên ngày được chọn
    const getAvailableShifts = () => {
        if (selectedWeekDay) {
            const usedShifts = courseData.shifts
                .filter((shift: IRowShift) => shift.weekDay === selectedWeekDay)
                .map((shift: IRowShift) => shift.shiftOfDay);
            return allShifts.filter(shift => !usedShifts.includes(shift));
        }
        return allShifts; // Nếu chưa chọn ngày, hiển thị tất cả các ca
    };

    const handleOk = () => {
        setLoading(true);
        form
        .validateFields({recursive: true})
        .then(async values => {
            console.log(values);
            const shiftDTO = {
                courseId: courseData.courseId,
                shiftOfDay: values.shiftOfDay,
                weekDay: values.weekDay,
                maxQuantity: values.maxQuantity
            }
            console.log(shiftDTO );
            try {
                const result = await postReq<any>('/Shifts', shiftDTO);
                if (result){
                    returnMessage( 'Add new shift successfully!', true);
                }
                onRefresh();
                form.resetFields();
                onCancelClick();
            }
            catch(error: any){
                console.log("Error while creating new shift: ", error)
                returnMessage(error.response.data, false);
            }

        })
        .catch(errorInfo => {
            console.log("Validation Failed:", errorInfo);
            returnMessage(errorInfo.errorFields[0].errors[0], false);
        });

        setLoading(false);
    };

    const handleCancel = () => {
        form.resetFields();
        onCancelClick();
    };

    return (
        <>
            <Modal className='modal' title="Create new shift" open={modalState} onOk={handleOk} onCancel={handleCancel} loading={loading}>
                <Divider/>
                <Form className='form'
                    form={form}
                    layout="horizontal"
                    labelCol={{ span: 8 }}
                    wrapperCol={{ span: 16 }}
                >
                    <Form.Item label="Day of week:" name="weekDay">
                        <Select onChange={value => setSelectedWeekDay(value)}>
                            {availableWeekDays.map(day => (
                                <Select.Option key={day} value={day}>{day}</Select.Option>
                            ))}
                        </Select>
                    </Form.Item>
                    <Form.Item label="Shift:" name="shiftOfDay" >
                        <Select>
                            {getAvailableShifts().map(shift => (
                                <Select.Option key={shift} value={shift}>{shift}</Select.Option>
                            ))}
                        </Select>
                    </Form.Item>
                    
                    <Form.Item label="Number of students:" name="maxQuantity">
                        <InputNumber min={20} max={40} />
                    </Form.Item>
                </Form>
                <Divider/>
            </Modal>
        </>
    );
}
export default ModalShift;