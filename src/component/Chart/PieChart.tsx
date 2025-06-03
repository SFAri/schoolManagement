import React, { useEffect } from 'react';
import * as echarts from 'echarts';

interface PieChartProps {
    title: string;
    subtext: string;
    data: { value: number; name: string }[];
}

const PieChart: React.FC<PieChartProps> = ({ title, subtext, data }) => {
    useEffect(() => {
        const dom = document.getElementById('chart-container') as HTMLElement;
        const myChart = echarts.init(dom, null, {
            renderer: 'canvas',
            useDirtyRect: false
        });

        const option: echarts.EChartsOption = {
            title: {
                text: title,
                subtext: subtext,
                left: 'center'
            },
            tooltip: {
                trigger: 'item'
            },
            legend: {
                orient: 'vertical',
                left: 'left'
            },
            series: [
                {
                    name: 'Number of students',
                    type: 'pie',
                    radius: '50%',
                    data: data,
                    emphasis: {
                        itemStyle: {
                            shadowBlur: 10,
                            shadowOffsetX: 0,
                            shadowColor: 'rgba(0, 0, 0, 0.5)'
                        }
                    },
                    label: { position: 'inside', formatter: '{d} %' },
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
    }, [title, data, subtext]);

    return <div id="chart-container" style={{ width: '500px', height: '400px' }} />;
};

export default PieChart;