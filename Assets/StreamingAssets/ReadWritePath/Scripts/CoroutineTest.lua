--开启一个协程
-- StartCoroutine(Function func)
--yield 函数
-- 1. yield.WaitForSeconds(float time)
-- 2. yield.WaitForEndOfFrame()


function myCoroutine1()
  print("myCoroutine1 run");
  for i = 0,100,1 do
     yield.WaitForSeconds(1)
     print("myCoroutine1_"..tostring(i))
   end

end

function myCoroutine2(a)
  print("myCoroutine2 run a="..tostring(a));
  for i = 0,100,1 do
     yield.WaitForSeconds(1)
     print("myCoroutine2_"..tostring(i))
   end
end

function myCoroutine3(a,b)
  print("myCoroutine3 run    ".."a="..tostring(a).." b="..tostring(b));
  for i = 0,100,1 do
     yield.WaitForSeconds(1)
     print("myCoroutine2_"..tostring(i))
   end
end


function Start()
   -- 无参
    StartCoroutine(myCoroutine1)
   -- 一个参数
    StartCoroutine(myCoroutine2,10) 
   -- 多个参数...
    StartCoroutine( myCoroutine3,"2222",19 )
end