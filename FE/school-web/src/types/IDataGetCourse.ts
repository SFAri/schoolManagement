import { IRowShift } from "./IRowShift"
import { IScore } from "./IScore"

export interface IDataGetCourse {
    courseId: number,
    courseName: string,
    startDate: string,
    endDate: string,
    lecturer: {
        lecturerId: string,
        lecturerName: string,
        email: string
    },
    shifts: IRowShift[],
    scores: IScore[],
}