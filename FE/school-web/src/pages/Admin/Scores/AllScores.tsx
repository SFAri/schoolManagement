import { useEffect, useRef, useState } from "react";
import App from "../../../App";
import { ColDef, ExcelExportParams, ExcelStyle, GridApi, GridReadyEvent } from "ag-grid-community";
import { IRowAllScore } from "../../../types/IRowAllScore";
import { DownloadOutlined } from "@ant-design/icons";
import { Form, message, Select, Spin, Tabs, TabsProps } from "antd";
import useMessage from "antd/es/message/useMessage";
import { ApiResult } from "../../../types/api/IApiResult";
import { getReq } from "../../../services/api";
import { IScore } from "../../../types/IScore";
import ButtonCustom from "../../../component/Button/Button";
import { AgGridReact } from "ag-grid-react";
import { ChartData } from "../DetailCourse/DetailCourse";
import LineChart from "../../../component/Chart/LineChart";

export interface GradeCount {
    name: string;  // Tên của grade (ví dụ: "A", "B", "C", ...)
    value: number; // Số lượng sinh viên đạt grade đó
}
interface StudentData {
    studentName: string;
    subjects: number;
    totalScore: number;
    averageScore: number;
    grade: string;
}
interface YearlyStudentData {
    [userId: string]: StudentData; // userId là chuỗi
}

export interface LineChartData {
    year: number;        // Năm (ví dụ: 2025)
    grades: GradeCount[];
}

const AllScores: React.FC<{role: string}> = ({role}) => {
    const [msg, contextHolder] = useMessage();
    const [rowData, setRowData] = useState<IRowAllScore[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [tabkey, setTabkey] = useState<Number>(1);
    const [chartData, setChartData] = useState<LineChartData[]>([]);
    const [selectedYear, setSelectedYear] = useState<Number>(2025);
    const didFetch = useRef(false);
    
    const [colMarkDefs, setColMarkDefs] = useState<ColDef<any>[]>([
        { field: "userId", hide: true},
        { headerName: "Course Name", field: "courseName" , filter: true},
        { headerName: "Student Name", field: "studentName", filter:true},
        { headerName: "Process 1", field: 'process1', filter: true}, // have to validate score here, range(0,10)
        { headerName: "Process 2", field: 'process2', filter: true},
        { headerName: "Midterm", field: 'midterm', filter: true},
        { headerName: "Final", field: 'final', filter: true},
        { headerName: "Average", field: 'averageScore', filter: true},
        { headerName: "Evaluation", field: 'grade', editable: false, valueFormatter: params => {
            return params.value.replace(/([A-Z])/g, ' $1').trim(); // Thêm khoảng trắng trước mỗi chữ cái hoa
        }},
    ]);

    const defaultColDef: ColDef = {
        flex: 1,
    };

    const defaultExcelExportParams: ExcelExportParams = {
        headerRowHeight: 30,
    };

    const calculateGrade = (averageScore: number): string => {
      if (averageScore >= 9.0) return 'Excellent';
      if (averageScore >= 8.0) return 'Very Good';
      if (averageScore >= 7.0) return 'Good';
      if (averageScore >= 6.0) return 'Average';
      if (averageScore < 6.0) return 'Below Average';
      return 'Not Graded';
  };

    // FETCH DATA HERE:
    const reloadData = async () => {
        setLoading(true);
        try {
          const {data, totalCount} = await getReq<ApiResult<IScore>>('/Scores');
          const mappedData: any[] = data.map(item => ({
            courseName: item.courseName,
            userId: item.userId,
            studentName : item.studentName,
            process1: item.process1,
            process2 : item.process2,
            midterm : item.midterm,
            final : item.final,
            averageScore : item.averageScore,
            grade: item.grade
          }));
          console.log("Scores: " + JSON.stringify(data, null, 2));
          setRowData(mappedData);
          const yearlyStudentData: { [year: number]: YearlyStudentData } = {};

          data.forEach(score => {
            const year = score.year; // Giả sử bạn có trường year trong score
            const userId = score.userId; // userId là chuỗi

            if (!yearlyStudentData[year]) {
                yearlyStudentData[year] = {};
            }

            if (!yearlyStudentData[year][userId.toLowerCase()]) {
              yearlyStudentData[year][userId.toLowerCase()] = {
                studentName: score.studentName,
                subjects: 0,
                totalScore: 0,
                averageScore: 0,
                grade: ''
              };
            }

            // Cập nhật số môn và tổng điểm
            yearlyStudentData[year][userId.toLowerCase()].subjects += 1;
            yearlyStudentData[year][userId.toLowerCase()].totalScore += score.averageScore; // Giả sử score.averageScore chứa điểm của môn học
          });

        // Tính điểm trung bình và grade cho từng sinh viên

        Object.keys(yearlyStudentData).forEach(year => {
            const students = yearlyStudentData[parseInt(year)];
            const gradesCount: { [grade: string]: number } = {};

            Object.keys(students).forEach(userId => {
                const student = students[userId];
                student.averageScore = student.totalScore / student.subjects; // Tính điểm trung bình

                // Gán grade dựa trên điểm trung bình
                student.grade = calculateGrade(student.averageScore);

                // Cập nhật số lượng sinh viên theo grade
                gradesCount[student.grade] = (gradesCount[student.grade] || 0) + 1;
            });

            // Chuyển đổi thành định dạng cho biểu đồ
            chartData.push({
                year: parseInt(year),
                grades: Object.keys(gradesCount).map(grade => ({
                    name: grade,
                    value: gradesCount[grade]
                }))
            });
        });
          console.log("Chart Data: ", JSON.stringify(chartData, null, 2));
          setChartData(chartData);
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
    },[]);

    const gridRef = useRef<AgGridReact<IRowAllScore>>(null);
    let gridApi: GridApi<IRowAllScore>;
    const onGridReady = (params: GridReadyEvent<IRowAllScore>) => {
        console.log('The grid is ready');
        // Example: Accessing the grid API
        const api = params.api;
        gridApi = params.api;
        api.sizeColumnsToFit();
    };
    const onExportClick=() =>{
        gridApi.exportDataAsExcel();
    }

    const excelStyles : ExcelStyle[] = [
    {
      id: "header",
      alignment: {
        vertical: "Center",
      },
      interior: {
        color: "#f8f8f8",
        pattern: "Solid",
        patternColor: undefined,
      },
      borders: {
        borderBottom: {
          color: "#ffab00",
          lineStyle: "Continuous",
          weight: 2,
        },

        borderRight: {
          color: "#ffab00",
          lineStyle: "Continuous",
          weight: 1,
        },
      },
    },
  ];

  const handleOnChange = (key: string) => {
    console.log(tabkey);
    setTabkey(parseInt(key));
  };
  const items: TabsProps['items'] = [
        {
            key: '1',
            label: 'All scores',
            children: [
                <>
                    <h2 className="title">All Scores</h2>
                      <div className="row-button">
                        <ButtonCustom 
                            label="Export To Excel"
                            iconButton = {<DownloadOutlined />}
                            onClickEvent={()=> onExportClick()}
                            colorButton={""}    
                        />
                      </div>
                    <div style={{height: 500}}>
                      <AgGridReact
                          ref = {gridRef}
                          rowData={rowData}
                          columnDefs={colMarkDefs}
                          defaultColDef={defaultColDef}
                          pagination={true}
                          paginationPageSize={10}
                          paginationPageSizeSelector={[10, 25, 50]}
                          onGridReady={onGridReady}
                          excelStyles={excelStyles}
                          defaultExcelExportParams={defaultExcelExportParams}
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
                  <div className="chart-container">
                    <LineChart data={chartData} title="Grading of students"/>
                  </div>
                </>
            ],
        },
    ];

    return (
        <>
            
            {contextHolder}
            <App selected="4" role={role}>
                {loading ? 
                    <div style={{ textAlign: 'center', marginTop: '50px' }}>
                        <Spin size="large" tip="Đang tải dữ liệu..." />
                    </div>
                : 
                    <>
                        <Tabs defaultActiveKey="1" items={items} onChange={handleOnChange} />
                    </>
                }
            </App>
        </>
    )
}

export default AllScores;