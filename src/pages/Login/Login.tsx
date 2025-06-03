/* eslint-disable react-hooks/rules-of-hooks */
import React, { useState } from "react"
import { LockOutlined, MailOutlined, UserOutlined } from '@ant-design/icons';
import { Button, Form, Input} from 'antd';
import './Login.css'
import { useAuth } from "../../hooks/useAuth";
import {  useNavigate } from "react-router-dom";
import useMessage from "antd/es/message/useMessage";
import { ROUTERS } from "../../utils/router";

const Login: React.FC = () => {
    const [message, contextHolder] = useMessage();
    const { login } = useAuth(); // Lấy hàm login từ context
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate(); // Sử dụng useNavigate để điều hướng

    const onFinish = async (values: any) => {
        setLoading(true); // Bắt đầu trạng thái loading

        try {
            await login(values.email, values.password); // Gọi hàm login
            message.success('Login successfully!'); // Thông báo thành công
            navigate(`/${ROUTERS.HOME}`); // Đường dẫn đến trang chính sau khi đăng nhập
        } catch (error : any) {
            message.error(`Login failed: ${error?.response?.data?.message}.`); // Thông báo lỗi
        } finally {
            setLoading(false); // Kết thúc trạng thái loading
        }
    };
    return (
        <>
            {contextHolder}
            <div className="login-container">
                <div className="left">
                    <UserOutlined style={{fontSize: '40px', color: 'white'}}/>
                </div>
                <div className="right">
                    <h2 className="title">Login</h2>
                    <Form
                        name="normal_login"
                        className="login-form"
                        initialValues={{ remember: true }}
                        onFinish={onFinish}
                    >
                        <Form.Item
                            name="email"
                            rules={[{type: "email", required: true, message: 'Please input your Email!' }]}
                        >
                            <Input prefix={<MailOutlined className="site-form-item-icon" />} placeholder="Email" />
                        </Form.Item>
                        <Form.Item
                            name="password"
                            rules={[{ required: true, message: 'Please input your Password!' }]}
                        >
                            <Input.Password
                                prefix={<LockOutlined className="site-form-item-icon" />}
                                type="password"
                                placeholder="Password"
                            />
                        </Form.Item>

                        <Form.Item>
                            <Button type="primary" htmlType="submit" className="login-form-button" loading={loading}>
                                Log in
                            </Button>
                        </Form.Item>
                    </Form>
                </div>
            </div>
        </>
    );
}

export default Login;