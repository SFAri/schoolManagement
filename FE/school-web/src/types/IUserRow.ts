import { EGender } from "./EGender";
import { ERole } from "./ERole";

export interface IRowUser {
    id: string,
    firstName: string,
    lastName: string,
    roleName: ERole,
    gender: EGender,
    email: string,
    dob: string
}