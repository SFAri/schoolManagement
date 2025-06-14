import { BackwardFilled, SaveFilled, ScheduleFilled } from "@ant-design/icons";
import App from "../../../App";
import ButtonCustom from "../../../component/Button/Button";
import { ColDef } from "ag-grid-community";
import { Suspense, useEffect, useRef, useState } from "react";
import { IRowShift } from "../../../types/IRowShift";
import { EShiftCode } from "../../../types/EShiftCode";
import { EWeekDay } from "../../../types/EWeekDay";
import { Button, Divider, message, Progress, Space, Spin, Tabs, TabsProps } from "antd";
import './DetailCourse.css'
import { useNavigate, useParams } from "react-router-dom";
import EditDeleteButton from "../../../component/EditDeleteButton/EditDeleteButton";
import CustomGridTable from "../../../component/GridTable/CustomGridTable";
import { IDataGetCourse } from "../../../types/IDataGetCourse";
import { FormatDate } from "../../../utils/formatDate";
import { deleteReq, getReq, patchReq } from "../../../services/api";
import { IScore } from "../../../types/IScore";
import { AgGridReact } from "ag-grid-react";
import ModalShift from "../../../component/ModalShift/ModalShift";
import dayjs from 'dayjs';
import { Spinner } from "react-bootstrap";
import PieChart from "../../../component/Chart/PieChart";
import BarChart from "../../../component/Chart/BarChart";
import { ScheduleXCalendar } from "@schedule-x/react";
import { createEventsServicePlugin } from "@schedule-x/events-service";
import { useCalendarApp } from "@schedule-x/react";
import { createViewWeek } from "@schedule-x/calendar";
import { createViewMonthGrid } from "@schedule-x/calendar";
import { createViewMonthAgenda } from "@schedule-x/calendar";
import { viewWeek } from "@schedule-x/calendar";
import { viewMonthGrid } from "@schedule-x/calendar";
import { createCalendarControlsPlugin } from '@schedule-x/calendar-controls'
import { createCalendar } from "@schedule-x/calendar";

export interface ChartData {
    name: string;
    value: number;
}

export const convertCourseToEvents = (course: IDataGetCourse) => {
    const events: { id: number; title: string; start: string; end: string }[] = [];
    if (course) {
        console.log("Course Name: " + course.courseName);
        const startDate = dayjs(course.startDate);
        const endDate = dayjs(course.endDate);
        const shifts = course.shifts;

        console.log("=== SHIFTS: " + JSON.stringify(shifts, null, 2));

        // Iterate through each day in the date range
        for (let date = startDate; date.isBefore(endDate) || date.isSame(endDate, 'day'); date = date.add(1, 'day')) {
            const weekDay = date.format('dddd');

            // Iterate through each shift
            shifts.forEach(shift => {
                if (shift.weekDay.toLowerCase() === weekDay.toLowerCase()) {
                    const startHour = shift.shiftOfDay === 'Morning' ? '07:00' : '13:00';
                    const endHour = shift.shiftOfDay === 'Morning' ? '12:00' : '18:00';
                    const start = date.format(`YYYY-MM-DD`) + ` ${startHour}`;
                    const end = date.format(`YYYY-MM-DD`) + ` ${endHour}`;
                    events.push({
                        id: shift.shiftId,
                        title: `${course.courseName} - ${shift.shiftOfDay}`,
                        start: start,
                        end: end
                    });
                }
            });
        }
    }
    return events;
};

const DetailCoursePage: React.FC<{role: string}> = ({role}) => {
    const navigator = useNavigate();
    let params = useParams();
    let id = params.id;
    const didFetch = useRef(false);
    const [data, setData] = useState<IDataGetCourse>();
    const [loading, setLoading] = useState<boolean>(true);
    const [msg, contextHolder] = message.useMessage();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const gridRef = useRef<AgGridReact>(null);
    const [myScore, setMyScore] = useState<IScore>();
    const [chartData, setChartData] = useState<ChartData[]>([]);
    const eventsServicePlugin = createEventsServicePlugin();
    const [events, setEvents] = useState<any>([]);
    const [selectedDate, setSelectedDate] = useState<string>('');
    const calendarControls = createCalendarControlsPlugin();

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

    // Fetch data here: Student chỉ coi được các ca học mà nó đã đăng ký học
    const [rowShiftData, setRowShiftData] = useState<IRowShift[]>([]);
    
      // Column Definitions: Defines & controls grid columns.
    const [colShiftDefs, setColShiftDefs] = useState<ColDef<IRowShift>[]>([
        { field: "shiftId", hide: true},
        { field: "weekDay"},
        { field: "shiftOfDay", filter:true},
        { field: "maxQuantity"}
    ]);

    // Data for mark: Nếu là student thì chỉ cho nó xem điểm của nó thôi -> fetch điểm của nó vào đây
    const [markData, setMarkData] = useState<any[]>([]);

    const [colMarkDefs, setColMarkDefs] = useState<ColDef<any>[]>([
        { field: "userId", hide: true},
        { headerName: "Student Name", field: "studentName", filter:true},
        { headerName: "Process 1", field: 'process1', editable: role === 'student'? false:true, }, // have to validate score here, range(0,10)
        { headerName: "Process 2", field: 'process2', editable: role === 'student'? false:true},
        { headerName: "Midterm", field: 'midterm', editable: role === 'student'? false:true},
        { headerName: "Final", field: 'final', editable: role === 'student'? false:true},
        { headerName: "Average", field: 'averageScore', editable: false, valueFormatter: p => p.value?.toFixed(2)},
        { headerName: "Grade", field: 'grade', editable: false, valueFormatter: params => {
            return params.value.replace(/([A-Z])/g, ' $1').trim(); // Thêm khoảng trắng trước mỗi chữ cái hoa
        }},
    ]);
    const defaultColDef: ColDef = {
        flex: 1,
    };

    const handleDelete = async (data: IRowShift) => {
        try {
            const result = await deleteReq<IRowShift>('/Shifts/' + data.shiftId);
            console.log('Shift deleted:', result);
            msg.success(`Delete Shift: ${data.weekDay} - ${data.shiftOfDay}`)
            reloadData();
        }
        catch(error: any){
            console.log("Error while deleting shift: ", error)
            msg.error(`Error while deleting Shift: ${error?.response?.data}`)
        }
    };

    const handleBack = ():void =>{
        navigator(-1);
    }

    // FETCH DATA HERE:
    const reloadData = async () => {
        setLoading(true);
        try {
            const result = await getReq<IDataGetCourse>('/Courses/' + id, {});
            if (result !== null){
                setData(result)
                setRowShiftData(result.shifts);
                let scores = result.scores.map((score : IScore) => {
                    return {
                        userId : score.userId,
                        studentName : score.studentName,
                        process1: score.process1,
                        process2: score.process2,
                        midterm: score.midterm,
                        final: score.final,
                        averageScore: score.averageScore,
                        grade: score.grade
                    }
                });
                let es = convertCourseToEvents(result);
                setEvents(es);

                es.forEach(event => {
                    eventsServicePlugin.add({
                        title: event.title,
                        start: event.start,
                        end: event.end,
                        id: event.id
                    });
                });
                setMarkData(scores);
                const gradeCounts = scores.reduce<{ [key: string]: number }>((acc, score) => {
                    acc[score.grade] = (acc[score.grade] || 0) + 1;
                    return acc;
                }, {});

                const chartData = Object.keys(gradeCounts).map((grade) => ({
                    name: grade,
                    value: gradeCounts[grade],
                }));

                setChartData(chartData);
                if (role === 'student'){
                    const userData = JSON.parse(localStorage.getItem('user') || '{}');
                    const currentUserId = userData.id;
                    const userScore = result.scores.find((score: IScore) => score.userId === currentUserId);
                    setMyScore(userScore);
                }

                let time: number = Date.parse(result.startDate);
                let res: Date = new Date(time);
                const year = res.getFullYear();
                const month = String(res.getMonth() + 1).padStart(2, '0'); // Thêm 1 vì tháng bắt đầu từ 0
                const day = String(res.getDate()).padStart(2, '0');
                const formattedDate = `${year}-${month}-${day}`;
                setSelectedDate(formattedDate);
            }
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

    // Calendar:
    const calendar = createCalendar({
        views: [
            createViewWeek(),
        ],
        selectedDate: selectedDate,
        defaultView: viewWeek.name,
        dayBoundaries: {
            start: '06:00',
            end: '18:00',
        },
        weekOptions: {
            gridHeight: 320,
        },
        events: events,
        plugins: [eventsServicePlugin, calendarControls],
    });

    useEffect(() => {
        if (!didFetch.current) {
            reloadData();
            didFetch.current = true;
        }
    },[]);

    useEffect(() => {
        if (selectedDate) {
            calendarControls.setDate(selectedDate); // ⚡ cập nhật selectedDate cho calendar
        }
    }, [calendarControls, selectedDate]);

    const onCellValueChanged = (params: any) => {
        const { data, colDef } = params;

        if (["process1", "process2", "midterm", "final"].includes(colDef.field)) {
            const { process1 = 0, process2 = 0, midterm = 0, final = 0 } = data;
            const avg = (
            (parseFloat(process1) * 0.1 +
                parseFloat(process2) * 0.2 +
                parseFloat(midterm) * 0.2 +
                parseFloat(final) * 0.5)
            ).toFixed(2);

            data.averageScore = Number(avg);
            data.grade = calculateGrade(Number(avg));
            data._edited = true; // Đánh dấu là đã chỉnh sửa
            params.api.applyTransaction({ update: [data] }); // Cập nhật lại hàng trong grid
        }
    };

    const calculateGrade = (averageScore: number): string => {
        if (averageScore >= 9.0) return 'Excellent';
        if (averageScore >= 8.0) return 'VeryGood';
        if (averageScore >= 7.0) return 'Good';
        if (averageScore >= 6.0) return 'Average';
        if (averageScore < 6.0) return 'BelowAverage';
        return 'NotGraded';
    };

    const handleUpdateClick = async () => {
        const editedRows = gridRef.current!.api
            .getRenderedNodes()
            .map(n => n.data)
            .filter(row => row._edited === true);

        console.log("Rows to update: ", editedRows);
        // CALL API BELOW:
        const scores = editedRows.map((row: any) => {
            return{
                courseId : id,
                userId : row.userId,
                process1 : row.process1,
                process2 : row.process2,
                midterm : row.midterm,
                final : row.final,
                average: row.averageScore,
                grade: row.grade
            }
        });

        try {
            await Promise.all(
                scores.map(async score => {
                    console.log ("score ==== " + JSON.stringify(score));
                    const res = await patchReq(`/Scores`, score);
                    console.log ("res ==== " + res);
                })
            );
            console.log("All scores updated successfully.");
            displayMessage("All scores updated successfully.", true);
            reloadData();
        } catch (error: any) {
            console.error("Error updating scores:", error);
            displayMessage("Error updating scores " + error?.message, false);
        }
    };

    // Tab:
    const onChange = (key: string) => {
        console.log(key);
    };
    
    const items: TabsProps['items'] = [
        {
            key: '1',
            label: 'Table scores',
            children: [
                <>
                    <h2 className="title">Scores of students</h2>
                    <CustomGridTable
                        gridRef={gridRef}
                        height={500}
                        rowData={markData}
                        colDefs={colMarkDefs}
                        defaultColDef={defaultColDef}
                        onCellValueChanged={onCellValueChanged} />
                        <div className="row-button-end">
                            <ButtonCustom
                                label="Save scores"
                                iconButton = {<SaveFilled />}
                                onClickEvent={handleUpdateClick}
                                colorButton={"#A0C878"}    
                            />
                        </div>
                </>
            ],
        },
        {
            key: '2',
            label: 'Chart',
            children: [
                <>
                    {chartData.length !== 0
                    ? 
                        <div className="chart-container">
                            {/* <h1>My ECharts Example</h1> */}
                            <PieChart data={chartData} subtext="Pie Chart" title="Rating of students"/>
                            <BarChart title="Rating of students" data={chartData} />
                        </div>

                    :
                        <div>
                            There are no scores to illustrate.
                        </div>
                    }
                    
                </>
            ],
        },
    ];

    return (
        <>
            {contextHolder}
            { loading ?
                (<div style={{ textAlign: 'center', marginTop: '50px' }}>
                    <Spin size="large" tip="Đang tải dữ liệu..." />
                </div>)
            :
            
                (<App role={role} selected="1">
                    <h2 className="title">{data?.courseName}</h2>
                    <div className="row-info">
                        {/* <Button type="primary" ghost disabled><b>Course Name:</b> {data?.courseName} </Button> */}
                        <Button type="primary" ghost disabled><b>Time:</b> {data && FormatDate(data?.startDate)} - {data && FormatDate(data?.endDate)}</Button>
                        <Button type="primary" ghost disabled><b>Lecturer:</b> {data?.lecturer.lecturerName}</Button>
                        <Button type="primary" ghost disabled><b>Year:</b> {data?.year}</Button>
                    </div>
                    <div className="row-button">
                        <ButtonCustom 
                            label="Back"
                            iconButton = {<BackwardFilled />}
                            onClickEvent={handleBack}
                            colorButton={""}    
                        />
                        {/* Admin function: add shift or delete */}
                        {role === 'admin' && dayjs(data?.startDate).startOf('day').isAfter(dayjs().startOf('day')) &&
                            <ButtonCustom 
                                label="Create shift"
                                iconButton={<ScheduleFilled />}
                                onClickEvent={toggleOpen} 
                                colorButton={"#328E6E"}                
                            />
                        }         
                    </div>

                    {/* If see grid */}
                    <h2 className="title">List Shifts</h2>
                    <div className="sx-react-calendar-wrapper" style={{ width: '1200px', height: '550px', overflow: "hidden" }}>
                        <ScheduleXCalendar calendarApp={calendar} />
                    </div>

                    {role === 'student' &&
                        <>
                            <Divider />
                            <h2 className="title">My score:</h2>
                            <div className="center-div">
                                <div className="div-left">
                                    <Progress type="circle" percent={(myScore?.averageScore ?? 0)*10} format={(percent) => `${(percent?percent:0)/10}`} />
                                </div>
                                
                                <div className="div-right">
                                    <p><b>Process 1: </b> {myScore?.process1} </p>
                                    <p><b>Process 2: </b> {myScore?.process2} </p>
                                    <p><b>Midterm: </b> {myScore?.midterm} </p>
                                    <p><b>Final: </b> {myScore?.final} </p>
                                    <p><b>Evaluation: </b> {myScore?.grade.replace(/([A-Z])/g, ' $1').trim()} </p>
                                </div>
                            </div>
                        </>
                    }
                    {/* score student: */}
                    {(role === 'admin' || role === 'lecturer') && 
                        <>
                            <Tabs defaultActiveKey="1" items={items} onChange={onChange} />
                            {isModalOpen && (
                                <Suspense fallback={<Spinner />}>
                                    <ModalShift modalState = {isModalOpen} onCancelClick = {toggleClose} courseData={data} onRefresh={reloadData} returnMessage={displayMessage} />
                                </Suspense>
                            )}
                        </>
                    }
                </App>)
            }
        </>
    );
}

export default DetailCoursePage;
