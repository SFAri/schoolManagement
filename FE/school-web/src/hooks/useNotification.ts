import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';
import { useEffect, useRef } from 'react';
import { notification } from 'antd';

const useNotification = (
    userId: string,
    onReceive: (data: any) => void
  ) => {
  const connectionRef = useRef<HubConnection | null>(null);

  useEffect(() => {
    if (!userId) return;

    const connection = new HubConnectionBuilder()
      .withUrl('https://localhost:7122/notification-hub', {
        accessTokenFactory: () => localStorage.getItem('token') || '',
      })
      .withAutomaticReconnect()
      .build();

    connectionRef.current = connection;

    connection
      .start()
      .then(async () => {
        console.log('🔗 SignalR connected');
      })
      .catch((err) => console.error('❌ SignalR connect error:', err));

    connection.on('ReceiveNotification', (data) => {
      console.log('🔔 Notification received:', data);

      // if (data && data.message && typeof data.message === 'string' && data.message.trim() !== '') {
        notification.info({
          message: '📢 New Notification',
          description: data.message,
          duration: 5,
        });
      // }
    });

    return () => {
      connection.stop();
    };
  }, [userId]);
};

export default useNotification;
