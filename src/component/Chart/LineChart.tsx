import React, { useEffect } from 'react';
import * as echarts from 'echarts';
import { GradeCount } from '../../pages/Admin/Scores/AllScores';

interface LineChartProps {
    title: string;
    data: { year: number;        // Năm (ví dụ: 2025)
    grades: GradeCount[] }[];
}

const LineChart: React.FC<LineChartProps> = ({ title, data }) => {
    useEffect(() => {
        const dom = document.getElementById('line-chart-container') as HTMLElement;
        const myChart = echarts.init(dom);

        const seriesData: echarts.SeriesOption[] = data.flatMap(item => 
            item.grades.map(grade => ({
                name: grade.name,
                type: 'line' as const, // Ensure type is recognized as 'line'
                stack: 'Total',
                data: data.map(d => {
                    const foundGrade = d.grades.find(g => g.name === grade.name);
                    return foundGrade ? foundGrade.value : 0; // Fallback to 0 if grade not found
                })
            }))
        );

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
                // name: 'Year',
                type: 'category',
                boundaryGap: false,
                data: data.map(item => item.year)
            },
            yAxis: {
                type: 'value',
                name: 'Students',
                position: 'left',
                minInterval: 1,
                maxInterval: 10,
            },
            // series: [
            //     {
            //         name: 'Email',
            //         type: 'line',
            //         stack: data.map(item => item.grades.map(grade => grade.name)),
            //         data: data.map(item => item.grades.map(grade => grade.value)),
            //         // data: [120, 132, 101, 134, 90, 230, 210]
            //     },
            // ]
            series: seriesData
        };

        

        if (option && typeof option === 'object') {
            myChart.setOption(option);
        }

        window.addEventListener('resize', () => {
            myChart.resize();
        });

        // Cleanup on component unmount
        return () => {
            window.removeEventListener('resize', () => {
                myChart.resize();
            });
            myChart.dispose();
        };
    }, [title, data]);

    return <div id="line-chart-container" style={{ width: '100%', height: '500px' }} />;
};

export default LineChart;