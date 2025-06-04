import { EGender } from "./EGender";

export interface IRowScoreDetail {
    id: number,
    firstName: string,
    lastName: string,
    gender: EGender,
    email: string,
    score: number
}