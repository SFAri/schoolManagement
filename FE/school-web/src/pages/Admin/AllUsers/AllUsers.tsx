import { Suspense, useEffect, useRef, useState } from "react";
import App from "../../../App";
import { AgGridReact } from 'ag-grid-react';
import { ColDef } from "ag-grid-community";
import { FolderAddFilled } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { ROUTERS } from "../../../utils/router";
import ButtonCustom from "../../../component/Button/Button";
import { IRowUser } from "../../../types/IUserRow";
import { ERole } from "../../../types/ERole";
import { EGender } from "../../../types/EGender";
import ModalUser from "../../../component/ModalUser/ModalUser";
import EditDeleteButton from "../../../component/EditDeleteButton/EditDeleteButton";
import { message, Spin } from "antd";
import dayjs from 'dayjs';
import { FormatDate } from "../../../utils/formatDate";
import { deleteReq, getReq } from "../../../services/api";
import { Spinner } from "react-bootstrap";

const AllUsersPage: React.FC = () => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [loading, setLoading] = useState<boolean>(true);
    const didFetch = useRef(false);
    const [msg, contextHolder] = message.useMessage();
    const navigator = useNavigate();
    function toggleOpen() {
        setIsModalOpen(!isModalOpen);
    }
    const toggleClose = () => {
        setIsModalOpen(false);
    }
    // ------------- FOR MESSAGE: ----------
    const displayMessage = (message : String, isSuccess : boolean) => {
        if (isSuccess){
            msg.success(message);
        }   
        else {
            msg.error(message);
        }
    }

    // FETCH DATA HERE:
    const reloadData = async () => {
        setLoading(true);
        try {
            const res = await getReq<any[]>('/User/all', { });
            let list: IRowUser[] = []
            res.forEach(element => {
                let user : IRowUser = {
                    ...element,
                    dob: FormatDate(element.dob)
                }
                list.push(user)
            });
            setRowData(list);
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
            reloadData();
            didFetch.current = true;
        }
    },[])

    // Row Data Interface
    const [rowData, setRowData] = useState<IRowUser[]>([]);
    
      // Column Definitions: Defines & controls grid columns.
    const [colDefs, setColDefs] = useState<ColDef<IRowUser>[]>([
        { field: "id", hide: true},
        { field: "firstName", filter:true},
        { field: "lastName", filter:true},
        { field: "roleName"},
        { field: "gender"},
        { field: "email"},
        { 
            field: "dob", 
            comparator: (a: string, b: string) => {
                const dateA = dayjs(a, 'DD/MM/YYYY').valueOf();
                const dateB = dayjs(b, 'DD/MM/YYYY').valueOf();
                return dateA - dateB; // Sort ascending
            }
        }
    ]);

    const defaultColDef: ColDef = {
        flex: 1,
    };

    const handleViewUser = (data: IRowUser) => {
        const id = data.id; 
        navigator('/' + ROUTERS.ADMIN.ALLUSERS + '/' + id);
    }

    // Call API here
    const handleDeleteUser = async (data: IRowUser) => {
        // console.log('data: ', data);
        // msg.success(`Delete user: ${data.firstName + ' ' + data.lastName} successfully!`);
        try {
            const result = await deleteReq<any>('/User/' + data.id);
            console.log('User deleted:', result);
            msg.success('Deleted ' + result);
            reloadData();
        }
        catch(error: any){
            console.log("Error while deleting: ", error)
            msg.error(`Error while deleting User: ${error}`)
        }
    }
      
    return (
        <>
            {contextHolder}
            <App role="admin" selected="2">
                <h2 className="title">List Users</h2>
                <div className="row-button">
                    <ButtonCustom 
                        label="Create user"
                        onClickEvent={toggleOpen}
                        iconButton = {<FolderAddFilled />}
                        colorButton=""
                    />
                </div>
                <div style={{ height: 500 }}>
                    {loading ? 
                        <div style={{ textAlign: 'center', marginTop: '50px' }}>
                            <Spin size="large" tip="Đang tải dữ liệu..." />
                        </div>
                    : 
                        <>
                        
                            <AgGridReact
                                rowData={rowData}
                                columnDefs={[...colDefs,
                                    {
                                        colId: "actionButton",
                                        field: 'button',
                                        headerName: 'Action',
                                        cellRenderer: EditDeleteButton,
                                        cellRendererParams: {
                                            enableEdit: false,
                                            onViewClicked: handleViewUser,
                                            onCancelClick: toggleClose,
                                            onConfirmDelete: handleDeleteUser
                                        }
                                    } 
                                ]}
                                defaultColDef={defaultColDef}
                                pagination={true}
                                paginationPageSize={10}
                                paginationPageSizeSelector={[10, 25, 50]}
                            />
                        </>
                    }
                </div>
                {isModalOpen && 
                    <Suspense fallback={<Spinner />}>
                        <ModalUser modalState = {isModalOpen} onCancelClick = {toggleClose} onRefresh={reloadData} returnMessage={displayMessage}/>
                    </Suspense>
                }
            </App>
        </>
    );
}

export default AllUsersPage;