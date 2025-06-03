import { IShiftInputCourse } from "./shiftModels";

export interface ICourseInput {
    courseName: string,
    lecturerId: string,
    startDate: string, 
    endDate: string, 
    shifts: IShiftInputCourse[]
}