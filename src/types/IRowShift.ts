import { EShiftCode } from "./EShiftCode";
import { EWeekDay } from "./EWeekDay";

export interface IRowShift {
    shiftId: number,
    shiftOfDay : EShiftCode,
    weekDay: EWeekDay,
    maxQuantity: number
}