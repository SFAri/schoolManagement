import { AntDesignOutlined } from "@ant-design/icons";
import { Button } from "antd";
import React from "react";
import './MissingPage.css'

export const MissingPage : React.FC = () => {
    return (
        <>
            <div className="center-div">
                <div className="div-left">
                    <img alt="" src="https://cdn.dribbble.com/userupload/42435577/file/original-3e9b7e1ec996e56d6418e2c36917733b.png?resize=1000x750&vertical=center" />
                </div>
                
                <div className="div-right">
                    <p className="p-style">
                        You're enter unauthorized or not found page.
                    </p> 
                    <Button type="primary" size="large" icon={<AntDesignOutlined />}>
                        Back to Homepage
                    </Button>
                </div>
            </div>
        </>
    )
} 