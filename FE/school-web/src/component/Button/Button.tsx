import { Button } from "antd";
import './Button.css';
import IButtonProps from "../../types/IButtonProps";


const ButtonCustom: React.FC<IButtonProps> = ({label, iconButton, onClickEvent, colorButton='blue'}) => {
    return (
        <Button className="custom-button" type="primary" shape="round" icon={iconButton} size='large' onClick={onClickEvent} style={{background: colorButton}}>
            {label}
        </Button>
    );
}

export default ButtonCustom;