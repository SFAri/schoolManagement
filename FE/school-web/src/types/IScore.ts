import { EGrade } from "./EGrade";

export interface IScore {
    courseName: String,
    year: number,
    userId: String,
    studentName: string,
    process1: number,
    process2: number,
    midterm: number,
    final: number,
    averageScore: number,
    grade: EGrade,
    createdAt: String,
    updatedAt: String
}