   M S B A M L   `   `   `  ����D  @ServerWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:ServerWPF	ServerWPF  5   	       ServerWPF.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:ServerWPF     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      �� 5      $
��Auto �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      �� 5      $
��Auto �6   .+�5   
   o� $��0��6   $��0��6   $��3��6,   
���   �$��1q�6T   .@�5      m� $
��Menu��6   
���   �$��1q�69   .@�5      m� $��
Disconnect��6   5      m� 5      $
��Help��6   
���   �$��1q�69   .@�5      m� -   5        $��Commands��5      6-   5      �� 5      $��Current room Id:��6   �� 5       -   5         ��Name$ 	txtRoomId��5       $
��Auto��6)   $
��0000��66   5!      o� 5#   
   $��1��6   $��0��6   
���   �$��1q�6A   .@�5%      m� -   5         m�Name$ 	menuRooms��5%      $��Rooms��6*   
���   �$��1q�6M   .@�5&      m� -   5        $ menuAddRoom��5&      $��	Add group��60   6C   �� 5'      5(      5)      �� 5+   
   $��1��6   $��1��6%   $��Stretch=�65   $��Stretch=�6S   .��5,      �� -   5        $ txtBlockChatWindow��5,      $��Stretch=�62   $��Stretch=�6P   
�������6l   5-      �� 5/   
   -   5        $ txtChat��5/      $��2��6#   $��0��60   $��2��6@   
���   �$��1q�6h   �� 51   
   -   5         ��Name$ 
btnRefresh��51      $��Refresh��6%   $��2��67   $��1��6G   
���   �$��1q�6h   6|   �� 52   
   -   5        $ btnSend��52      $
��Send��6"   $��2��61   $��2��6A   
���   �$��1q�6b   6v   54      55      