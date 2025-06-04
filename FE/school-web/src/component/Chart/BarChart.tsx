import React, { useEffect } from 'react';
import * as echarts from 'echarts';

interface BarChartProps {
    title: string;
    data: { name: string; value: number }[];
}

const BarChart: React.FC<BarChartProps> = ({ title, data }) => {
    useEffect(() => {
        const dom = document.getElementById('bar-chart-container') as HTMLElement;
        const myChart = echarts.init(dom);

        const option: echarts.EChartsOption = {
            title: {
                text: title,
                subtext: 'Bar Chart',
                left: 'center'
            },
            tooltip: {
                trigger: 'item'
            },
            xAxis: {
                name: 'Grade',
                type: 'category',
                data: data.map(item => item.name)
            },
            yAxis: {
                type: 'value',
                name: 'Students',
                position: 'left',
                minInterval: 1,
                maxInterval: 5,
                // min: 0,
                // max: 40
            },
            series: [
                {
                    barWidth: '30%',
                    type: 'bar',
                    data: data.map(item => item.value),
                    emphasis: {
                        itemStyle: {
                            color: '#ff7f50'
                        }
                    }
                }
            ]
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

    return <div id="bar-chart-container" style={{ width: '600px', height: '400px' }} />;
};

export default BarChart;