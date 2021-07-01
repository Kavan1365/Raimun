# Raimun

این برنامه یا ابزارهای

sqlserver , docker, RabbitMQ,sawgger,serilog ,hangfire




مرحله اول بدون داکر:

تغییر کانکشن استرینگ به 
       
       "SqlServer": "Data Source=.;Initial Catalog=RaimunDbContext;Integrated Security=true"

در مسیر Webapi

dotnet run =>

swagger=> https://localhost:5001/swagger/index.html
hangfire=>https://localhost:5001/hangfire
مسیر نمایش لاگ در 
WebApi 
هست


دوتا کنترلر به بنام
user, weather
درست کردم 
user=> 
دوتا اکشن داره یکی 
token , create 
هست
 weather => گرفتن اطلاعات دما شهرستان سنندج و استقاده از بروکر و 
 hangfire 
در صورتی که بعد از ده دقیقه دیگه متد
SendMyJobForEmail 
اجرا بشه 
در بروکر دیتا ارسال میشه و اگه دمای ان بیشتر از 14 بود دیتابیس ذخیره بشه 




