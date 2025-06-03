import { Avatar, DatePicker, Divider, Form, Input, message, Select, Spin } from "antd";
import App from "../../App";
import { CloseOutlined, SaveFilled, UserOutlined } from "@ant-design/icons";
import './DetailUser.css';
import { ERole } from "../../types/ERole";
import { EGender } from "../../types/EGender";
import ButtonCustom from "../../component/Button/Button";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useRef, useState } from "react";
import { ColDef } from "ag-grid-community";
import { AgGridReact } from "ag-grid-react";
import { getReq, patchReq } from "../../services/api";
import { IRowUser } from "../../types/IUserRow";
import dayjs, { Dayjs } from 'dayjs';
import { IScore } from "../../types/IScore";


const DetailUser: React.FC<{role: string, selected: string}> = ({role, selected}) => {
    const navigator = useNavigate();
    const params = useParams();
    const id = params.id;
    const didFetch = useRef(false);
    const [loading, setLoading] = useState<boolean>(false);
    const [msg, contextHolder] = message.useMessage();
    const [dataUser, setDataUser] = useState<IRowUser>();
    const [form] = Form.useForm();
    // Fetch data here:
    const fetchUserData = async () => {
        setLoading(true);
        try {
            let string = id === undefined ? '/User/me' : `/User/${id}`;
            const result = await getReq<any>(string, id!==undefined? params : {});
            console.log("===RES: " + JSON.stringify(result));
            const mappedData: any[] = result.scores.map((score : IScore) => {
                return {
                        userId : score.userId,
                        courseName : score.courseName,
                        process1: score.process1,
                        process2: score.process2,
                        midterm: score.midterm,
                        final: score.final,
                        averageScore: score.averageScore,
                        grade: score.grade
                    }
            });
            var user : any = {
                ...result,
                dob : dayjs(result.dob)
            }
            setDataUser(user);
            setRowData(mappedData);
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
    // const {data, loading, error} = useFetch<IRowUser>(id === undefined ? '/User/me' : `/User/${id}`, id === undefined ? {} : params);

    useEffect(() => {
        if (!didFetch.current) {
            fetchUserData();
            didFetch.current = true;
        }
    },[]);

    const displayMessage = (message : String, isSuccess : boolean) => {
        if (isSuccess){
            msg.success(message);
        }   
        else {
            msg.error(message);
        }
    }

    const [rowData, setRowData] = useState<any[]>([]);
    
    const [colMarkDefs, setColMarkDefs] = useState<ColDef<any>[]>([
        { field: "userId", hide: true},
        { headerName: "Course Name", field: "courseName", filter:true},
        { headerName: "Process 1", field: 'process1', editable: role === 'student'? false:true, }, // have to validate score here, range(0,10)
        { headerName: "Process 2", field: 'process2', editable: role === 'student'? false:true},
        { headerName: "Midterm", field: 'midterm', editable: role === 'student'? false:true},
        { headerName: "Final", field: 'final', editable: role === 'student'? false:true},
        { headerName: "Average", field: 'averageScore', editable: false, valueFormatter: p => p.value?.toFixed(2)},
        { headerName: "Evaluation", field: 'grade', editable: false, valueFormatter: params => {
            return params.value.replace(/([A-Z])/g, ' $1').trim(); // Thêm khoảng trắng trước mỗi chữ cái hoa
        }},
    ]);

    const defaultColDef: ColDef = {
        flex: 1,
    };

    // const handleOnChange = ():void =>{

    // }
    const handleSave = ():void =>{
        setLoading(true);
        form
        .validateFields({recursive: true})
        .then(async values => {
            console.log(values);
            console.log("dataUser: ", dataUser)
            console.log("dayJS: ", dayjs('2003-03-22T00:00:00').format('DD/MM/YYYY'))
            const patchUser :any = {};
            if (values.firstName !== dataUser?.firstName){
                patchUser.firstName = values.firstName;
            }
            if (values.lastName !== dataUser?.lastName){
                patchUser.lastName = values.lastName;
            }
            if (values.dob.format('YYYY-MM-DD') !== dayjs(dataUser?.dob).format('YYYY-MM-DD')){
                const DOBString = values.dob.format('YYYY-MM-DD') + 'T00:00:00Z';
                console.log("from data:", dataUser?.dob)
                patchUser.dob = DOBString;
            }
            if (values.gender !== dataUser?.gender){
                patchUser.gender = values.gender;
            }
            if (values.roleName !== dataUser?.roleName){
                patchUser.roleType = values.roleName;
            }
            if (values.password !== undefined && values.password !== ''){
                patchUser.password = values.password;
            }

            console.log("PatchUser: ", patchUser);
            if (Object.keys(patchUser).length === 0 ){
                displayMessage('Nothing to change', false);
            }
            else {
                try {
                    await patchReq<IRowUser>('/User/' + dataUser?.id, patchUser);
                    form.setFieldsValue( { password: undefined } )
                    displayMessage('Update user information successfully!', true);
                    fetchUserData();
                }
                catch(error: any){
                    console.log("Error while creating new user: ", error)
                }
            }
        })
        .catch(errorInfo => {
            console.log("Validation Failed:", errorInfo);
            displayMessage(errorInfo.errorFields[0].errors[0], false);
        });
        setLoading(false);
    }
    const handleBack = ():void =>{
        navigator(-1);
    }

    const handleOnChange = (date: Dayjs, dateString:string|string[]) => {
        console.log('Selected Date:', date);
        console.log('Formatted Date String:', dateString);
      };

    // selected={location.pathname.includes(ROUTERS.ADMIN.ALLUSERS)? '2' : '3'}
    return (
        <>
            {contextHolder}
            <App role={role} selected={selected}>
                {loading ? 
                        <div style={{ textAlign: 'center', marginTop: '50px' }}>
                            <Spin size="large" tip="Đang tải dữ liệu..." />
                        </div>
                    : 
                        <>
                        
                            <div className="row-info">
                                <Avatar size={80} icon={<UserOutlined />} />
                            </div>
                            <Form
                                className='form'
                                form={form}
                                initialValues={dataUser}
                            >
                                <div className="block-info">
                                    <div className="block-left">
                                        <Form.Item name="firstName" label="First name:" rules={[{ required: true }]} labelCol={{ span: 5 }}>
                                            <Input />
                                        </Form.Item>
                                        <Form.Item name="email" label="Email" rules={[{ type: 'email' , required: true}]} labelCol={{ span: 5 }}>
                                            <Input />
                                        </Form.Item>
                                        <Form.Item name="roleName" label="Role:"  rules={[{ required: true }]} labelCol={{ span: 5 }}>
                                            <Select placeholder='Select role' disabled={role !== "admin" || dataUser?.roleName === ERole.Admin}>
                                                {id === null && 
                                                    <Select.Option value={ERole.Admin}>{ERole.Admin}</Select.Option>
                                                } 
                                                <Select.Option value={ERole.Lecturer}>{ERole.Lecturer}</Select.Option>
                                                <Select.Option value={ERole.Student}>{ERole.Student}</Select.Option>
                                            </Select>
                                        </Form.Item>
                                        {role === "admin" && 
                                            <Form.Item name="password" label="Reset password:" labelCol={{ span: 5 }}>
                                                <Input />
                                            </Form.Item>
                                        }
                                    </div>
                                    <div className="block-right">
                                        <Form.Item name="lastName" label="Last name:" rules={[{ required: true }]} labelCol={{ span: 5 }}>
                                            <Input />
                                        </Form.Item>
                                        <Form.Item name="dob" label="DOB:" labelCol={{ span: 5 }} rules={[{ required: true, message: 'Please select a valid birthday' }]}>
                                            <DatePicker  maxDate={dayjs()} onChange={handleOnChange} format={'DD/MM/YYYY'} title='DOB' style={{ width: '100%' }} />
                                        </Form.Item>
                                        <Form.Item name="gender" label="Gender:" rules={[{ required: true }]} labelCol={{ span: 5 }}>
                                            <Select placeholder='Select Gender'>
                                                <Select.Option value={EGender.Female}>{EGender.Female}</Select.Option>
                                                <Select.Option value={EGender.Male}>{EGender.Male}</Select.Option>
                                            </Select>
                                        </Form.Item>
                                    </div>
                                </div>
                                <div className="row-button-end">
                                    <ButtonCustom
                                        label="Update"
                                        iconButton = {<SaveFilled />}
                                        onClickEvent={handleSave}
                                        colorButton={"#A0C878"}    
                                    />
                                    <ButtonCustom
                                        label="Cancel"
                                        iconButton = {<CloseOutlined/>}
                                        onClickEvent={handleBack}
                                        colorButton={"gray"}    
                                    />
                                </div>
                            </Form>

                            {/* CASE 01: Lecturer => grid all courses that this teacher have */}

                            {/* CASE 02: Student =>  grid all score of this student */}
                            {role === "student" && (
                                <>
                                    <Divider />
                                    <h2 className="title">Scores of students</h2>
                                    <div style={{height: 400}}>
                                        <AgGridReact
                                            rowData={rowData}
                                            columnDefs={colMarkDefs}
                                            defaultColDef={defaultColDef}
                                            pagination={true}
                                            paginationPageSize={10}
                                            paginationPageSizeSelector={[10, 25, 50]}
                                        />
                                    </div>
                                </>
                            )}
                        </>}
            </App>
        </>
    )
}

export default DetailUser;