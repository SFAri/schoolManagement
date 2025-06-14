import React, { useEffect } from 'react';
import * as echarts from 'echarts';
import { GradeCount } from '../../pages/Admin/Scores/AllScores';

interface LineChartProps {
    title: string;
    data: { year: string;        // Năm (ví dụ: 2025)
    grades: GradeCount[] }[];
}

const LineChart: React.FC<LineChartProps> = ({ title, data }) => {
    useEffect(() => {
        const dom = document.getElementById('line-chart-container') as HTMLElement;
        const myChart = echarts.init(dom);

        // const seriesData: echarts.SeriesOption[] = data.flatMap(item => 
        //     item.grades.map(grade => ({
        //         name: grade.name,
        //         type: 'line' as const, // Ensure type is recognized as 'line'
        //         stack: 'Total',
        //         data: data.map(d => {
        //             const foundGrade = d.grades.find(g => g.name === grade.name);
        //             return foundGrade ? foundGrade.value : 0; // Fallback to 0 if grade not found
        //         })
        //     }))
        // );

        // const option: echarts.EChartsOption = {
        //     title: {
        //         text: title,
        //         subtext: 'Line Chart',
        //         left: 'center'
        //     },
        //     tooltip: {
        //         trigger: 'axis'
        //     },
        //     grid: {
        //         left: '3%',
        //         right: '4%',
        //         bottom: '3%',
        //         containLabel: true
        //     },
        //     toolbox: {
        //         feature: {
        //         saveAsImage: {}
        //         }
        //     },
        //     xAxis: {
        //         // name: 'Year',
        //         type: 'category',
        //         boundaryGap: false,
        //         data: data.map(item => item.year)
        //     },
        //     yAxis: {
        //         type: 'value',
        //         name: 'Students',
        //         position: 'left',
        //         minInterval: 1,
        //         maxInterval: 10,
        //     },
        //     series: seriesData
        // };

        

        // if (option && typeof option === 'object') {
        //     myChart.setOption(option);
        // }

        // window.addEventListener('resize', () => {
        //     myChart.resize();
        // });

        // // Cleanup on component unmount
        // return () => {
        //     window.removeEventListener('resize', () => {
        //         myChart.resize();
        //     });
        //     myChart.dispose();
        // };
        const allGradeNames = Array.from(
            new Set(data.flatMap(item => item.grades.map(grade => grade.name)))
        );

        const seriesData: echarts.SeriesOption[] = allGradeNames.map(gradeName => ({
            name: gradeName,
            type: 'line',
            // stack: 'Total',
            showSymbol: true, // hiện dấu chấm rõ ràng cả khi bằng 0
            data: data.map(item => {
                const grade = item.grades.find(g => g.name === gradeName);
                return grade ? grade.value : 0;
            })
        }));

        const option: echarts.EChartsOption = {
            title: {
            text: title,
            subtext: 'Line Chart',
            left: 'center'
            },
            tooltip: {
            trigger: 'axis'
            },
            grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
            },
            toolbox: {
            feature: {
                saveAsImage: {}
            }
            },
            xAxis: {
            type: 'category',
            boundaryGap: false,
            data: data.map(item => item.year)
            },
            yAxis: {
            type: 'value',
            name: 'Students',
            minInterval: 1
            },
            series: seriesData
        };

        myChart.setOption(option);
        const resizeFn = () => myChart.resize();
        window.addEventListener('resize', resizeFn);

        return () => {
            window.removeEventListener('resize', resizeFn);
            myChart.dispose();
        };
    }, [title, data]);

    return <div id="line-chart-container" style={{ width: '100%', height: '500px' }} />;
};

export default LineChart;