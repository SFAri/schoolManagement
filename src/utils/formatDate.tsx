
export const FormatDate = (date : string|undefined) : string => {
    if (date === undefined){
        const time = new Date('2025-03-10T02:00:00Z');
        const res = (time.getDate() < 10 ? `0${time.getDate()}` : time.getDate()) +'/' + (time.getMonth() + 1 < 10 ? `0${time.getMonth()+1}` : time.getMonth()+1) + '/'+time.getFullYear();
        return res;
    }
    let time: number = Date.parse(date);
    let res: Date = new Date(time);
    const dateFormat = `${res.getDate() < 10 ? `0${res.getDate()}` : res.getDate()}/${res.getMonth()+1 < 10 ? `0${res.getMonth()+1}` : res.getMonth()+1}/${res.getFullYear()}`;
    return dateFormat;
}