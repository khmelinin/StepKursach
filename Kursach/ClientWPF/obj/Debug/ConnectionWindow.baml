   M S B A M L   `   `   `  ����D  @ClientWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:ClientWPF	ClientWPF  5   	        ClientWPF.ConnectionWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:ClientWPF     5   	     � Title$  ConnectionWindow��5   	   $	��128��6"   $	��380��6/    � ShowInTaskbar .   � 
ResizeMode$ NoResize=�6Q    � WindowStartupLocation$ CenterOwner=�6g    � WindowStyle$ 
ToolWindow=�6�   .��5	      �  � RowDefinitions 5
   
   �� 5      �� 5      �� 5       � ColumnDefinitions 5   
   �� 5      �� 5      .+�5   
   �� $��0��6   $��0��6!   $��Adress��61   �� 5   
   -   5         ��Name$ txtIpAdress��5   /   $��0��6   $��1��6   $��	127.0.0.1��6D   �� 5   
   $��1��6   $��0��6!   $
��Port��61   �� 5   
   -   5        $ txtPort��5   /   $��1��6   $��1��6   $
��8888��6@   �� 5   
   -   5         ��Name$ 	btnCreate��5   .   $��2��6   $��0��6   .��6Y   Go6]   �� 5   
   -   5        $ 	btnCancel��5   .   $��2��6   $��1��6   .��6Y   Cancel6a   5      5      