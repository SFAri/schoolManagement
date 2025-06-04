import * as React from 'react';
import "antd/dist/reset.css";
import "./index.css";
import { Button, Layout, Menu, MenuProps} from "antd";
import './assets/styles/global.css';
import { AllCommunityModule, ModuleRegistry } from 'ag-grid-community'; 
import { ApartmentOutlined, BookFilled, LockFilled, LogoutOutlined, NumberOutlined, ProfileFilled, ScheduleFilled, UserOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { ROUTERS } from './utils/router';
import { LicenseManager, TreeDataModule, ExcelExportModule } from "ag-grid-enterprise";
import { Props } from './types/Props';
import ModalChangePass from './component/ModalChangePass/ModalChangePass';
import { useAuth } from './hooks/useAuth';
import useMessage from 'antd/es/message/useMessage';


// Register all Community features
ModuleRegistry.registerModules([AllCommunityModule, TreeDataModule, ExcelExportModule]);
LicenseManager.setLicenseKey("[TRIAL]_this_{AG_Charts_and_AG_Grid}_Enterprise_key_{AG-086015}_is_granted_for_evaluation_only___Use_in_production_is_not_permitted___Please_report_misuse_to_legal@ag-grid.com___For_help_with_purchasing_a_production_key_please_contact_info@ag-grid.com___You_are_granted_a_{Single_Application}_Developer_License_for_one_application_only___All_Front-End_JavaScript_developers_working_on_the_application_would_need_to_be_licensed___This_key_will_deactivate_on_{14 May 2025}____[v3]_[0102]_MTc0NzE3NzIwMDAwMA==1127112b2eb5fda550f75b9d26691ef0");

const {Sider, Header, Content, Footer} = Layout;

const App: React.FC<Props> = ({children, role, selected="1"}) => {
  const navigate = useNavigate();
  const [msg, contextHolder] = useMessage(); 
  const {logout} = useAuth();
  const user = JSON.parse(localStorage.getItem("user")!);
  
  const [isModalOpen, setIsModalOpen] = React.useState(false);
  function toggleOpen() {
      setIsModalOpen(!isModalOpen);
  }
  const toggleClose = () => {
      setIsModalOpen(false);
  }

  // ------------- FOR MESSAGE: ----------
  const displayMessage = (message : String, isSuccess : boolean) => {
    if (isSuccess){
        msg.success(message);
    }   
    else {
        msg.error(message);
    }
}
  
  type MenuItem = Required<MenuProps>['items'][number];
  
  // Login -> set item base on role
  let items: MenuItem[];

    switch (role) {
      case 'admin':
        items = [
          { key: '1', icon: <BookFilled />, label: 'Courses', onClick: () => {navigate('/'+ROUTERS.ADMIN.ALLCOURSES)} }, // Have to set here base on role thì sẽ nav đến page khác
          { key: '2', icon: <ApartmentOutlined />, label: 'Users', onClick: () => {navigate('/'+ROUTERS.ADMIN.ALLUSERS)}},
          { key: '3', icon: <ProfileFilled />, label: 'My Profile', onClick: () => {navigate('/'+ROUTERS.PROFILE)}},
          { key: '4', icon: <NumberOutlined />, label: 'Scores', onClick: () => {navigate('/'+ROUTERS.ADMIN.ALLSCORES)}},
        ]
        break;
      
      case 'lecturer':
        items = [
          { key: '1', icon: <BookFilled />, label: 'Courses', onClick: () => {navigate('/'+ROUTERS.LECTURER.MYCOURSE)} }, // Have to set here base on role thì sẽ nav đến page khác
          { key: '2', icon: <ScheduleFilled />, label: 'Schedule', onClick: () => {navigate('/'+ROUTERS.LECTURER.MYSCHEDULE)} }, 
          { key: '3', icon: <ProfileFilled />, label: 'My Profile', onClick: () => {navigate('/'+ROUTERS.LECTURER.MYPROFILE)}},
        ]
        break;
      default:
        items = [
          { key: '1', icon: <BookFilled />, label: 'Courses', onClick: () => {navigate('/'+ROUTERS.STUDENT.MYCOURSE)} }, // Have to set here base on role thì sẽ nav đến page khác
          { key: '2', icon: <ScheduleFilled />, label: 'Schedule', onClick: () => {navigate('/'+ROUTERS.STUDENT.MYSCHEDULE)} }, 
          { key: '3', icon: <ProfileFilled />, label: 'My Profile', onClick: () => {navigate('/'+ROUTERS.STUDENT.MYPROFILE)}},
        ]
        break;
    }
    
  const handleLogout = async() => {
    try {
        await logout(); // Gọi hàm login
        msg.success('Logout successfully!'); // Thông báo thành công
        navigate(`/${ROUTERS.LOGIN}`); // Đường dẫn đến trang chính sau khi đăng nhập
    } catch (error : any) {
        msg.error(`Logout failed! ${error.message}.`); // Thông báo lỗi
    }
  }

  return(
    <>
      {contextHolder}
      <Layout style={{minHeight: '100vh'}}>
        <Sider
          theme='dark'
          breakpoint="lg"
          collapsedWidth="0"
          onBreakpoint={(broken) => {}}
          onCollapse={(collapsed, type) => {}}
        >
          {/* <div className="logo" /> */}
          <div className='user-banner'>
            <div className='user-icon'>
              <UserOutlined style={{color: 'rgba(255, 255, 255, 0.65)', fontSize:'30px'}}/>
            </div>

            {/* Have to set here base on login info from users */}
            <div className='user-info'>
              <div className='user-name'>{user.firstName + ' ' + user.lastName}</div>
              <div className='user-role'>{user.roleName}</div>
            </div>
          </div>
          <Menu
            theme='dark'
            mode="inline"
            selectedKeys={[selected]}
            items={items}
          />
        </Sider>
        <Layout>
          <Header
            className="site-layout-sub-header-background"
          >
            <Button type='dashed' className='logout-button' onClick={handleLogout}><LogoutOutlined/> Logout</Button>
            <Button type='dashed' className='change-pass' onClick={toggleOpen}><LockFilled/>Change password</Button>
          </Header>
          <Content style={{ margin: "24px 16px 0" }}>
            <div
              className="site-layout-background"
              style={{ padding: 24, minHeight: 360 }}
            >
              {children}
            </div>
          </Content>
          <Footer style={{ textAlign: "center" }}>
            Ant Design ©2018 Created by Ant UED
          </Footer>

          <ModalChangePass modalState = {isModalOpen} onCancelClick = {toggleClose} returnMessage={displayMessage}/>
        </Layout>
      </Layout>
    </>
  )
}

export default App;
