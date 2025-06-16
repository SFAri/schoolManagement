import React, { useState, useEffect } from 'react';
import { Badge, Popover, List, Spin, Typography, Button, Tooltip } from 'antd';
import { BellFilled, CheckCircleOutlined } from '@ant-design/icons';
import { getReq, putReq } from '../../services/api';
import useNotification from '../../hooks/useNotification';

interface Notification {
  id: string;
  userId: string;
  message: string;
  createdAt: string;
  isRead: boolean;
}

const NotificationBell: React.FC<{ userId: string }> = ({ userId }) => {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [visible, setVisible] = useState(false);
  const [loading, setLoading] = useState(false);
  const [unreadCount, setUnreadCount] = useState(0);

  const pageSize = 100000;
  const page = 1;

  const fetchUnreadCount = async () => {
    try {
      const res = await getReq<{ unreadCount: number }>(
        `/Notification/unread-count?userId=${userId}`,
        {}
      );
      setUnreadCount(res.unreadCount);
    } catch (err) {
      console.error('Error fetching unread count', err);
    }
  };

  const fetchNotifications = async () => {
    try {
      setLoading(true);
      const res = await getReq<any>(
        `/Notification?userId=${userId}&page=${page}&pageSize=${pageSize}`,
        {}
      );
      setNotifications(res.data);
    } catch (err) {
      console.error('Failed to fetch notifications', err);
    } finally {
      setLoading(false);
    }
  };

  const markAsRead = async (id: string) => {
    try {
      await putReq(`/Notification/${id}/read`, {});
      setNotifications((prev) =>
        prev.map((noti) => (noti.id === id ? { ...noti, isRead: true } : noti))
      );
      fetchUnreadCount(); // cập nhật badge
    } catch (err) {
      console.error('Error marking as read', err);
    }
  };

  // Real-time: lắng nghe signalR
  useNotification(userId, (newNotification) => {
    setNotifications((prev) => [newNotification, ...prev]);
    setUnreadCount((prev) => prev + 1);
  });

  const handleVisibleChange = (newVisible: boolean) => {
    setVisible(newVisible);
    if (newVisible) {
      fetchNotifications();
      fetchUnreadCount();
    }
  };

  const content = (
    <div style={{ width: 320, maxHeight: 400, overflowY: 'auto' }}>
      {loading ? (
        <Spin />
      ) : (
        <List
          dataSource={notifications}
          locale={{ emptyText: 'Không có thông báo nào.' }}
          renderItem={(item) => (
            <List.Item
              style={{
                background: item.isRead ? '#fff' : '#f6ffed',
                padding: '10px 15px',
                marginBottom: 8,
                border: '1px solid #f0f0f0',
                borderRadius: 6,
              }}
              actions={
                !item.isRead
                  ? [
                        <>
                            <Tooltip title="prompt text">
                                <Button
                                    type="link"
                                    size="small"
                                    onClick={() => markAsRead(item.id)}
                                >
                                    <CheckCircleOutlined />
                                </Button>
                            </Tooltip>
                        </>,
                    ]
                  : undefined
              }
            >
              <List.Item.Meta
                title={<Typography.Text>{item.message}</Typography.Text>}
                description={new Date(item.createdAt).toLocaleString()}
              />
            </List.Item>
          )}
        />
      )}
    </div>
  );

  useEffect(() => {
    fetchUnreadCount(); // Load ngay khi mount
  }, [userId]);

  return (
    <Popover
      content={content}
      trigger="click"
      placement="bottomRight"
      open={visible}
      onOpenChange={handleVisibleChange}
    >
      <span style={{ display: 'flex', alignItems: 'center', height: '100%' }}>
        <Badge count={unreadCount} overflowCount={99}>
          <BellFilled style={{ fontSize: '20px', cursor: 'pointer' }} />
        </Badge>
      </span>
    </Popover>
  );
};

export default NotificationBell;