import { Button, message, Popconfirm, PopconfirmProps } from "antd";
import { DeleteOutlined, EditOutlined, EyeFilled } from "@ant-design/icons";


const EditDeleteButton: React.FC<{
    onViewClicked: (data: any) => void;
    toggleOpenModal: (data: any) => void;
    onConfirmDelete: (data:any) => void;
    enableEdit: boolean;
    enableView: boolean;
    enableDelete: boolean
    data?: any; // Thêm props để nhận dữ liệu hàng
}> = ({ toggleOpenModal, onViewClicked, data, enableEdit = true, onConfirmDelete, enableView = true , enableDelete = true}) => {
    
    const handleDelete = (): void => {
        onConfirmDelete(data)
    };

    // const confirm: PopconfirmProps['onConfirm'] = (e) => {
    //     console.log(e);
    //     message.success('Click on Yes');
    // };

    return (
        <div>
            {enableView &&
                <Button className="custom-button" shape="round" icon={<EyeFilled style={{color: 'blue'}}/>} onClick={() => onViewClicked(data)} style={{background: 'white'}}/>
            }

            {enableEdit &&
                <Button className="custom-button" shape="round" icon={<EditOutlined style={{color: 'green'}}/>} onClick={() => toggleOpenModal(data)} style={{background: 'white'}}/>
            } 

            {enableDelete && 
                 <Popconfirm
                 title="Delete data"
                 description="Are you sure to delete?"
                 onConfirm={()=> onConfirmDelete(data)}
                 okText="Confirm"
                 cancelText="Cancel"
                >
                    <Button className="custom-button" shape="round" icon={<DeleteOutlined style={{color: 'red'}}/>} style={{background: 'white'}}/>
                </Popconfirm>
            }
        </div>
    );
}

export default EditDeleteButton;