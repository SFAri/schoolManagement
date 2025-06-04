import { DatePicker, Divider, Form, Input, Modal, Select, SelectProps } from 'antd';
import React from 'react';
import dayjs from 'dayjs';
import { ERole } from '../../types/ERole';
import { EGender } from '../../types/EGender';
import { postReq } from '../../services/api';
import { IRowUser } from '../../types/IUserRow';
// import './ModalCourse.css';

const ModalUser: React.FC<any> = ({modalState, onCancelClick, onRefresh, returnMessage}) => {
    const [form] = Form.useForm();
    // Option here is mixed of WeekDay + ShiftCode (VD: "Monday - Morning", "Tuesday - Afternoon" ,...)
    const options: SelectProps['options'] = [];
    for (let i = 10; i < 36; i++) {
        options.push({
            label: i.toString(36) + i,
            value: i.toString(36) + i,
        });
    }

    const handleOnChange = () => {

    }

    // Validate and Call Api
    const handleOk = () => {
        form
        .validateFields({recursive: true})
        .then(async values => {
            console.log(values);
            // Convert to Date object
            // const convertDOB = new Date(values.dob.$d);
            // Convert to ISO string
            const DOBString = values.dob.format('YYYY-MM-DD') + 'T00:00:00Z';
            const registerDTO = {
                firstName: values.firstName.toString(),
                lastName: values.lastName.toString(),
                email: values.email.toString(),
                DOB: DOBString,
                gender: values.gender,
                roleType: values.roleName,
                password: values.roleName + '123#'
            }
            console.log(registerDTO);
            try {
                const result = await postReq<IRowUser>('/User/register', registerDTO);
                if (result){
                    returnMessage( 'Add new user successfully!', true);
                }
                onRefresh();
                onCancelClick();
            }
            catch(error: any){
                console.log("Error while creating new user: ", error)
                returnMessage(error.message, false);
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

    return (
        <>
            <Modal className='modal' title="Create new user" open={modalState} onOk={handleOk} onCancel={handleCancel}>
                <Divider/>
                <Form className='form'
                    form={form}
                    labelCol={{ span: 5 }}
                    wrapperCol={{ span: 18 }}
                    layout="horizontal"
                >
                    <Form.Item label="First name:" name="firstName" rules={[{ required: true, message: 'First name cannot be empty' }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item label="Last name:" name="lastName"  rules={[{ required: true, message: 'Last name cannot be empty'  }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item label="Role:" name="roleName" rules={[{ required: true }]}>
                        <Select placeholder='Select role'>
                            <Select.Option value={ERole.Lecturer}>{ERole.Lecturer}</Select.Option>
                            <Select.Option value={ERole.Student}>{ERole.Student}</Select.Option>
                        </Select>
                    </Form.Item>
                    <Form.Item name="email" label="Email" rules={[{ type: 'email', message: 'Please enter a valid email', required: true }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item label="DOB:" name="dob" rules={[{ required: true, message: 'Please select a valid birthday'  }]}>
                        <DatePicker 
                            // minDate={dayjs().subtract(100, 'year')}
                            format={'DD/MM/YYYY'}
                            maxDate={dayjs()} 
                            onChange={handleOnChange} title='DOB'
                        />
                    </Form.Item>
                    <Form.Item label="Gender:" name="gender" rules={[{ required: true, message: 'Please select a gender' }]}>
                        <Select placeholder='Select Gender'>
                            <Select.Option value={EGender.Female}>{EGender.Female}</Select.Option>
                            <Select.Option value={EGender.Male}>{EGender.Male}</Select.Option>
                        </Select>
                    </Form.Item>
                </Form>
                <Divider />
            </Modal>
        </>
    );
}
export default ModalUser;