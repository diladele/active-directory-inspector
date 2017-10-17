rem wget http://192.168.1.103:8000/ip/list/ --save-headers
rem  wget http://192.168.1.103:8000/ip/lookup/192.168.1.103 --save-headers
wget http://192.168.1.103:8000/ip/lookup/192.168.0.0 --save-headers --content-on-error=on
pause
