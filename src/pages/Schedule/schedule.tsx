import App from "../../App";
import './style.css'
import { useCalendarApp, ScheduleXCalendar } from '@schedule-x/react';
import {
  createViewWeek,
  createViewMonthGrid,
  createViewMonthAgenda,
} from '@schedule-x/calendar';
import { createEventsServicePlugin } from '@schedule-x/events-service';
import '@schedule-x/theme-default/dist/index.css';
import { useEffect, useRef, useState } from "react";
import { IDataGetCourse } from "../../types/IDataGetCourse";
import { getReq } from "../../services/api";
import { ApiResult } from "../../types/api/IApiResult";
import { Spin } from "antd";
import dayjs from 'dayjs';

const SchedulePage: React.FC<{role: string}> = ({role}) => {
    const eventsServicePlugin = createEventsServicePlugin();
    const [events, setEvents] = useState<any>([]);
    const didFetch = useRef(false);
    const [loading, setLoading] = useState<boolean>(true);

    const convertCoursesToEvents = (courses: IDataGetCourse[]) => {
        const events: { id: any; title: string; start: string; end: string; }[] = [];

        if (Array.isArray(courses)) {
            console.log("==== IS ARRAY!!!! ===")
            courses.forEach((course : IDataGetCourse) => {
                console.log("CourseName :" + course.courseName);
                const startDate = dayjs(course.startDate);
                const endDate = dayjs(course.endDate);
                const shifts = course.shifts; // Giả sử shifts là thuộc tính của course
                console.log("===SHIFTS: " + JSON.stringify(shifts, null, 2));
    
                // Lặp qua từng ngày trong khoảng thời gian
                for (let date = startDate; date.isBefore(endDate) || date.isSame(endDate, 'day'); date = date.add(1, 'day')) {
                    const weekDay = date.format('dddd');
                    
    
                    // Lặp qua từng ca học
                    shifts.forEach(shift => {
                        if (shift.weekDay.toLowerCase() === weekDay.toLowerCase()) {
                            console.log("-weekday:"+weekDay);
                            const startHour = shift.shiftOfDay === 'Morning' ? '07:00' : '13:00';
                            const endHour = shift.shiftOfDay === 'Morning' ? '12:00' : '18:00';

                            
                            const start = date.format(`YYYY-MM-DD`) + ` ${startHour}`;
                            const end = date.format(`YYYY-MM-DD`) + ` ${endHour}`;

                            console.log('--start:' + start);
                            console.log('--end:' + end);
                            events.push({
                                id: shift.shiftId,
                                title: `${course.courseName} - ${shift.shiftOfDay}`,
                                start :start,
                                end: end
                            });
                        }
                    });
                }
            });
        }
        console.log("EVENTS: ", events)
        return events;
    };
    
    const reloadData = async () => {
        setLoading(true);
        try {
            const { data } = await getReq<ApiResult<IDataGetCourse>>('/Courses', { pageNumber: 1, pageSize: 10 });
            if (data !== null) {
                const es = convertCoursesToEvents(data);
                setEvents(es);
                // console.log("Events after reload: ", es);

                es.forEach(event => {
                    eventsServicePlugin.add({
                        title: event.title,
                        start: event.start,
                        end: event.end,
                        id: event.id
                    });
                });
            }
        } catch (error: any) {
            console.error("Lỗi:", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (!didFetch.current) {
            reloadData();
            didFetch.current = true;
        }
    }, []);

    const calendar = useCalendarApp({
        views: [
            createViewWeek(),
            createViewMonthGrid(),
            createViewMonthAgenda(),
        ],
        dayBoundaries: {
            start: '06:00',
            end: '18:00',
        },
        weekOptions: {
            gridHeight: 450
        },
        events: events,
        plugins: [eventsServicePlugin],
    });
    
    
    return (
        <>
            { loading ?
                (<div style={{ textAlign: 'center', marginTop: '50px' }}>
                    <Spin size="large" tip="Đang tải dữ liệu..." />
                </div>)
            : (
                <App selected={"2"} role={role}>
                    <div className="sx-react-calendar-wrapper" style={{ width: '1200px', height: '800px', overflow: "hide" }}>
                        <ScheduleXCalendar calendarApp={calendar} />
                    </div>
                </App>
            )}
        </>
    );
}

export default SchedulePage;