import { useEffect, useRef, useState, lazy, Suspense, useMemo } from "react";
import App from "../../../App";
import { AgGridReact } from 'ag-grid-react';
import { ColDef} from "ag-grid-community";
import { BookOutlined, FolderAddFilled } from "@ant-design/icons";
import './AllCourses.css'
import { IRowCourse } from "../../../types/IRowCourse";
import dayjs from 'dayjs';
import { useNavigate } from "react-router-dom";
import { ROUTERS } from "../../../utils/router";
import ButtonCustom from "../../../component/Button/Button";
import EditDeleteButton from "../../../component/EditDeleteButton/EditDeleteButton";
import { message, Spin } from "antd";
import { FormatDate } from "../../../utils/formatDate";
import { Spinner } from "react-bootstrap";
import { deleteReq, getReq, postReq } from "../../../services/api";
import { IDataGetCourse } from "../../../types/IDataGetCourse";
import { ApiResult } from "../../../types/api/IApiResult";
import Swal from 'sweetalert2';
const  ModalCourse = lazy(() =>import( "../../../component/ModalCourse/ModalCourse"));
const ModalJoinCourse = lazy(() => import('../../../component/ModalJoinCourse/ModalJoinCourse'));
const ModalEditCourse = lazy(() => import('../../../component/ModalCourse/ModalEditCourse'));

const AllCoursesPage: React.FC<{role: string}> = ({role}) => {
    const [msg, contextHolder] = message.useMessage();
    const [totalPage, setTotalPage] = useState<number>(1);
    const [loading, setLoading] = useState<boolean>(true);
    const didFetch = useRef(false);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isModalEditOpen, setIsModalEditOpen] = useState(false);
    const navigator = useNavigate();
    const [courseEdit, setCourseEdit] = useState<any>(null);
    

    // ------------- FOR MESSAGE: ----------
    const displayMessage = (message : String, isSuccess : boolean) => {
        if (isSuccess){
            msg.success(message);
        }   
        else {
            msg.error(message);
        }
    }
    // ------------- FOR MODAL: -----------
    function toggleOpen() {
        setIsModalOpen(!isModalOpen);
    }
    const toggleClose = () => {
        setIsModalOpen(false);
    }

    // Modal edit:
    const toggleEditOpen = (data: IRowCourse) => {
        setCourseEdit(data);
        setIsModalEditOpen(!isModalEditOpen);
    }
    
    const toggleEditClose = () => {
        setIsModalEditOpen(false);
    }


    // FETCH DATA HERE:
    const reloadData = async () => {
        setLoading(true);
        try {
            const {data, totalCount} = await getReq<ApiResult<IDataGetCourse>>('/Courses', { pageNumber: 1, pageSize: 10 });
            console.log('Dữ liệu từ API:', data);
            const mappedData: IRowCourse[] = data.map(item => ({
                courseId: item.courseId,
                courseName: item.courseName,
                lecturerName: item.lecturer.lecturerName,
                startDate: item.startDate,
                endDate: item.endDate,
                year: item.year
            }));
            setTotalPage(totalCount);
            setRowData(mappedData);
        } catch (error: any) {
            if (error.name === 'CanceledError') {
                console.log('Request canceled: ', error.response?.data);
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

    // ----------------- FOR AGGRID : -----------------------
    const [rowData, setRowData] = useState<IRowCourse[]>([]);
    
    const defaultColDef: ColDef = {
        flex: 1,
    };

    const autoGroupColumnDef = useMemo<ColDef>(() => {
        return {
        minWidth: 200,
        };
    }, []);
    
      // Column Definitions: Defines & controls grid columns.
    const [colDefs, setColDefs] = useState<ColDef<IRowCourse>[]>([
        {
            field: 'year',
            rowGroup: true,
            hide: true,
        },
        { field: "courseId", hide: true},
        { field: "courseName", filter:true},
        { field: "lecturerName", filter:true},
        { field: "startDate", 
            cellRenderer: (params: any) => {
                return params.node.group ? '' : FormatDate(params.value);
            }
        },
        { field: "endDate", 
            cellRenderer: (params: any) => {
                return params.node.group ? '' : FormatDate(params.value);
            }
        },
        // {
        //     colId: "actionButton",
        //     field: 'button',
        //     headerName: 'Action',
        //     cellRenderer: (params: any) => {
        //         if (params.node.group) return null;
        //         return EditDeleteButton(params);
        //     },
        //     cellRendererParams: (params: { data: { startDate: string } }) => ({
        //         toggleOpenModal: toggleEditOpen,
        //         onViewClicked: handleViewCourse,
        //         onCancelClick: toggleClose,
        //         onConfirmDelete: handleDelete,
        //         enableDelete: isDeleteEnabled(params.data.startDate),
        //         enableEdit: role === 'admin'
        //     })
        // }
    ]);

    // -------------------------
    const handleViewCourse = (data: IRowCourse) => {
        const id = data.courseId;

        switch (role) {
            case 'admin':
                navigator('/' + ROUTERS.ADMIN.ALLCOURSES + '/' + id);
                break;
            case 'lecturer':
                navigator('/' + ROUTERS.LECTURER.MYCOURSE + '/' + id);
                break;
            default:
                navigator('/' + ROUTERS.STUDENT.MYCOURSE + '/' + id);
                break;
        }
    };

    const handleDelete = async (data: IRowCourse) => {
        switch (role) {
            case 'admin':
                try {
                    const result = await deleteReq<IDataGetCourse>('/Courses/' + data.courseId);
                    console.log('Course deleted:', result);
                    msg.success(`Delete Course: ${data.courseName}`)
                    reloadData();
                }
                catch(error: any){
                    console.log("Error while deleting: ", error)
                    msg.error(`Error while deleting Course: ${error}`)
                }
                break;
        
            // Student
            default:
                try {
                    const result = await deleteReq<IDataGetCourse>('/Courses/unjoin/' + data.courseId);
                    console.log('Unjoin Course:', result);
                    msg.success(`Unjoin Course: ${data.courseName}`)
                    reloadData();
                }
                catch(error: any){
                    console.log("Error while unjoining: ", error)
                    msg.error(`Error while unjoining Course: ${error}`)
                }
                break;
        }
        
    };

    const isDeleteEnabled = (startDate: string) => {
        const start = dayjs(startDate).startOf('day');
        const now = dayjs().startOf('day');
        return (role === 'admin' || (role === 'student' && start.isAfter(now)));
    };

    const showAlert = async () => {
        const getLast = await getReq<any>('/AcademicYear', {});
        if (getLast!== null){
            const result = await Swal.fire({
                title: 'Warning!',
                text: 'Are you sure ending up ' + getLast.year + ' to start a new academic year. Cautions: You cannot modified any courses or relevant information of ended academic year?',
                icon: 'warning',
                confirmButtonText: 'Confirm',
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
            });
    
            if (result.isConfirmed) {
                try {
                    // Gọi API để xóa
                    let result: any = await postReq('/AcademicYear', {});
                    console.log('res: ' + result.year);
                    Swal.fire(
                        'End old year!',
                        'You have just ended an old academic year and start a new academic year(' + result.year + ') successfully!',
                        'success'
                    );
                } catch (error) {
                    Swal.fire(
                        'Lỗi!',
                        'Có lỗi xảy ra khi xóa mục.',
                        'error'
                    );
                }
            }
        }
    };

      
    return (
        <>
            {contextHolder}
            <App role={role} selected="1">
                        <h2 className="title">List Courses</h2>
                        <div className="row-button">
                            { role === 'admin' && (
                                <>
                                    <ButtonCustom 
                                        label="Create course"
                                        onClickEvent={toggleOpen}
                                        iconButton = {<FolderAddFilled />}
                                        colorButton=""
                                    />

                                    <ButtonCustom 
                                        label="End Year"
                                        onClickEvent={showAlert}
                                        iconButton = {<BookOutlined />}
                                        colorButton="green"
                                    />
                                </>
                            )}
                            { role === 'student' && (
                                <ButtonCustom 
                                    label="Join course"
                                    onClickEvent={toggleOpen}
                                    iconButton = {<FolderAddFilled />}
                                    colorButton=""
                                />
                            )}
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
                                        columnDefs =  {[...colDefs, 
                                                {
                                                    colId: "actionButton",
                                                    field: 'button',
                                                    // hide: true,
                                                    headerName: 'Action',
                                                    // cellRenderer: EditDeleteButton,
                                                    cellRenderer: (params: any) => {
                                                        if (params.node.group) return null; // Không render gì cho group row
                                                        return EditDeleteButton(params);    // Render nút bình thường cho row con
                                                    },
                                                    cellRendererParams: (params: { data: { startDate: string; }; }) => ({
                                                        toggleOpenModal: toggleEditOpen,
                                                        onViewClicked: handleViewCourse,
                                                        onCancelClick: toggleClose,
                                                        onConfirmDelete: handleDelete,
                                                        enableDelete: isDeleteEnabled(params.data?.startDate),
                                                        enableEdit: role === 'admin'
                                                    })
                                                } 
                                            ]
                                        }
                                        defaultColDef={defaultColDef}
                                        pagination={true}
                                        paginationPageSize={10}
                                        groupDefaultExpanded={0} // Mở tất cả các nhóm
                                        rowGroupPanelShow={"always"}
                                        animateRows={true}
                                        autoGroupColumnDef={autoGroupColumnDef}
                                        // paginationAutoPageSize={true}
                                        paginationPageSizeSelector={[10, 25, 50]}
                                    />
                                </>
                            }
                        </div>
                        {role === 'admin' && 
                        <>
                            {(isModalOpen || isModalEditOpen) && (
                                <>
                                    <Suspense fallback={<Spinner />}>
                                        <ModalCourse modalState={isModalOpen} onCancelClick={toggleClose} onRefresh={reloadData} returnMessage={displayMessage}/>
                                    </Suspense>
                                    <Suspense fallback={<Spinner />}>
                                        <ModalEditCourse modalState = {isModalEditOpen} onCancelClick = {toggleEditClose} course= {courseEdit} onRefresh={reloadData} returnMessage={displayMessage}/>
                                    </Suspense>
                                </>
                            )}
                        </>
                        }
                        {role === 'student' && 
                            <>
                                {isModalOpen && (
                                    <Suspense fallback={<Spinner />}>
                                        <ModalJoinCourse modalState = {isModalOpen} onCancelClick = {toggleClose} onRefresh={reloadData} returnMessage={displayMessage} />
                                    </Suspense>
                                )}
                            </>
                        }
                
            </App>
        </>
    );
}

export default AllCoursesPage;