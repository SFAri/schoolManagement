export interface IModalProp {
    modalState: boolean;
    onCancelClick: ()=>void;
    onRefresh: ()=>void;
    returnMessage: (message:string, isSuccess: boolean)=>void;
}