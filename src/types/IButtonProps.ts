export default interface IButtonProps {
    label:string;
    iconButton: React.ReactNode;
    colorButton?: string;
    onClickEvent : ()=>void;
};