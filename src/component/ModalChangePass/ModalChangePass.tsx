import { Divider, Form, Input, Modal } from "antd";
import { postReq } from "../../services/api";

const ModalChangePass: React.FC<any> = ({modalState, onCancelClick, returnMessage}) => {
    const [form] = Form.useForm();
    const user = JSON.parse(localStorage.getItem("user")!);
    const onFinish = (values: any) => {
        console.log('Received values of form: ', values);
    };
    const handleOk = () => {
        form
        .validateFields({recursive: true})
        .then(async values => {
            console.log(values);
            const changePswDTO = {
                oldPassword : values.oldPassword,
                newPassword: values.newPassword
            }
            console.log(changePswDTO);
            try {
                const result = await postReq<any>('/User/reset-password/' + user.id, changePswDTO);
                if (result){
                    returnMessage( 'Change password successfully!', true);
                }
                form.resetFields();
                onCancelClick();
            }
            catch(error: any){
                console.log("Error while changing password: ", error?.response?.data)
                returnMessage(error?.response?.data, false);
            }
        
        })
        .catch(errorInfo => {
            // // ❌ lỗi validate:
            console.log("Validation Failed:", errorInfo);
            returnMessage(errorInfo.errorFields[0].errors[0], false);
        })
    };

    const handleCancel = () => {
        form.resetFields();
        onCancelClick();
    };
    return (
        <>
            <Modal className='modal' title="Change password" open={modalState} onOk={handleOk} onCancel={handleCancel}>
                <Divider/>
                <Form
                    form={form}
                    name="changePassword"
                    onFinish={onFinish}
                    style={{ maxWidth: 600 }}
                    labelCol={{ span: 8 }}
                    wrapperCol={{ span: 16 }}
                    scrollToFirstError
                >
                    <Form.Item
                        name="oldPassword"
                        label="Old Password"
                        rules={[
                        {
                            required: true,
                            message: 'Please input your password!',
                        },
                        ]}
                        hasFeedback
                    >
                        <Input.Password />
                    </Form.Item>

                    <Form.Item
                        name="newPassword"
                        label="New Password"
                        rules={[
                        {
                            required: true,
                            message: 'Please input your password!',
                        },
                        ]}
                        hasFeedback
                    >
                        <Input.Password />
                    </Form.Item>

                    <Form.Item
                        name="confirm"
                        label="Confirm Password"
                        dependencies={['newPassword']}
                        hasFeedback
                        rules={[
                        {
                            required: true,
                            message: 'Please confirm your password!',
                        },
                        ({ getFieldValue }) => ({
                            validator(_, value) {
                            if (!value || getFieldValue('newPassword') === value) {
                                return Promise.resolve();
                            }
                            return Promise.reject(new Error('The new password that you entered do not match!'));
                            },
                        }),
                        ]}
                    >
                        <Input.Password />
                    </Form.Item>
                </Form>
            </Modal>
        </>
    )
}

export default ModalChangePass;