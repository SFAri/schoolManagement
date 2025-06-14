/* eslint-disable react-hooks/rules-of-hooks */
import React, { useState } from "react"
import { LockOutlined, LoginOutlined, MailOutlined, UserOutlined } from '@ant-design/icons';
import { Button, Form, Input} from 'antd';
import './Login.css'
import { useAuth } from "../../hooks/useAuth";
import {  useNavigate } from "react-router-dom";
import useMessage from "antd/es/message/useMessage";
import { ROUTERS } from "../../utils/router";
import logo_login from '../../assets/images/login.jpeg';

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
                    <img className="logo-login" src={logo_login} alt="Logo" />
                    {/* <UserOutlined style={{fontSize: '40px', color: 'white'}}/> */}
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
                            <Input size="large" prefix={<MailOutlined className="site-form-item-icon" />} placeholder="Email" />
                        </Form.Item>
                        <Form.Item
                            name="password"
                            rules={[{ required: true, message: 'Please input your Password!' }]}
                        >
                            <Input.Password
                                size="large"
                                prefix={<LockOutlined className="site-form-item-icon" />}
                                type="password"
                                placeholder="Password"
                            />
                        </Form.Item>

                        <Form.Item>
                            <Button size="large" type="primary" htmlType="submit" className="login-form-button" loading={loading}>
                                Log in 
                                <LoginOutlined />
                            </Button>
                        </Form.Item>
                    </Form>
                    <p className="hint-text-login">*Note: You must enter provided account to use this website</p>
                </div>
            </div>
        </>
    );
}

export default Login;