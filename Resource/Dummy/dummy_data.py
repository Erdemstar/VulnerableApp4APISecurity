import requests
import json
import random
import string
import ccard
import sys

class RequestHelper():

    def __init__(self):
        self.proxy = {"http":"http://localhost:8080"}
        #self.proxy = ""
   
    def GET(self,url,jwt:None):
        if jwt is None:
            x = requests.get(url,headers={"Content-Type": "application/json"},
                            verify=False,
                            proxies=self.proxy)
            return x.text
        else:
            x = requests.get(url,headers={"Content-Type": "application/json","Authorization": "Bearer " + jwt},
                            verify=False,
                            proxies=self.proxy)
            return x.text

    def POST(self,url,jwt,data):
        if jwt is None:
            x = requests.post(url,headers={"Content-Type": "application/json"},
                              data=json.dumps(data),
                              verify=False,
                              proxies=self.proxy)
            return x.text
        else:
            x = requests.post(url,headers={"Content-Type": "application/json","Authorization": "Bearer " + jwt},
                                data=json.dumps(data),
                                verify=False,
                                proxies=self.proxy)
            return x.text

    def File_Upload(self,url,jwt,file):
        x = requests.post(url,headers={"Authorization": "Bearer " + jwt},
                            files=file,
                            verify=False,
                            proxies=self.proxy)
        return x.text

    def PUT(self,url,jwt,data):
        x = requests.put(url,headers={"Content-Type": "application/json","Authorization": "Bearer " + jwt},
                          data=json.dumps(data),
                          verify=False,
                          proxies=self.proxy)
        return x.text

    def DELETE(self,url,jwt,data:None):
        if data is None:
            x = requests.delete(url,headers={"Content-Type": "application/json","Authorization": "Bearer " + jwt},
                            verify=False,
                            proxies=self.proxy)
            return x.text
        else:
            x = requests.delete(url,headers={"Content-Type": "application/json","Authorization": "Bearer " + jwt},
                            data=json.dumps(data),
                            verify=False,
                            proxies=self.proxy)
            return x.text

class RandomDataHelper():
    def __init__(self):
        pass
    def string(self):
        r = random.randint(15,25)
        return ''.join(random.choice(string.ascii_letters + string.digits) for _ in range(r))
    
    def integer(self,r):
        return ''.join(random.choice(string.digits+ string.digits) for _ in range(r)) 
    
    def userGenerate(self):
        name = self.string()
        surname = self.string()
        password = self.string()
        email = name + surname + "@gmail.com"
        return name,surname,password,email
    
    def profileGenerate(self):
        hobby = self.string()
        address = self.string()
        date = "2023-07-18T12:46:07.806Z"
        return hobby,address,date

    def cardGenerate(self):
        card_name = self.string()
        card_number = str(ccard.visa())
        card_date = "12/25"
        card_cve = self.integer(3)
        card_pass = self.string()
        return card_name,card_number,card_date,card_cve,card_pass

class VulnerableApp4API():

    def __init__(self,base_url):
        self.request_helper = RequestHelper()
        self.base_url = base_url
        self.token = ""
        self.refrestToken = ""

    # ACCOUNT
    def account_login(self,email,password):
        try:
            self.console_log(1, "Account Login request is sent")
            PATH = "/api/Account/Login"
            data = {"email":email ,"password":password}
            result = self.request_helper.POST(self.base_url+PATH,None,data)
            self.token = json.loads(result)["accessToken"]
            self.refrestToken = json.loads(result)["refreshToken"]
            self.console_log(1,self.token + self.refrestToken)
        except:
            self.console_log(2, "Account Login request is failed while sent")

    def account_temporaray_login(self,email,password):
        try:
            self.console_log(1, "Account Temporary Login request is sent")
            PATH = "/api/Account/TemporaryLogin?Email=" + email + "&Password=" + password
            result = self.request_helper.GET(self.base_url+PATH,None)
        except:
            self.console_log(2, "Account Temporary Loginrequest is failed while sent")

    def account_register(self,name,surname,email,password):
        try:
            self.console_log(1, "Account Login request is sent")
            PATH = "/api/Account/Register"
            data = {"email":email ,"password":password,"name":name,"surname":surname}
            result = self.request_helper.POST(self.base_url+PATH,None,data)
            self.console_log(1,result)
        except:
            self.console_log(2, "Account Register request is failed while sent")

    def account_refreshtoken(self):
        try:
            self.console_log(1, "RefreshToken request is sent")
            PATH = "/api/Account/RefreshToken"
            data = {"refreshToken":self.refrestToken}
            result = self.request_helper.POST(self.base_url+PATH,self.token,data)
            self.token = json.loads(result)["accessToken"]
            self.refrestToken = json.loads(result)["RefreshToken"]
            self.console_log(1,self.token)
        except:
            self.console_log(2, "RefreshTokenrequest is failed while sent")

    def account_tokenstatus(self):
        try:
            self.console_log(1, "TokenStatus Login request is sent")
            PATH = "/api/Account/TokenStatus"
            result = self.request_helper.GET(self.base_url+PATH,self.token)
            self.console_log(1,result)
        except Exception as er:
            print (er)
            self.console_log(2, "TokenStatusrequest is failed while sent")
    
    # USER
    def user_get(self,email):
        try:
            self.console_log(1, "Account Get request is sent")
            PATH = "/api/User/?Email=" + email
            result = self.request_helper.GET(self.base_url + PATH,self.token)
            self.console_log(1,result)
        except:
            self.console_log(2, "Account Get request is failed while sent")
            
    def user_update(self,email,name,surname,password):
        try:
            self.console_log(1, "Account Update request is sent")
            PATH = "/api/User/?Email=" + email
            data = {"name":name,"surname":surname,"password":password}
            result = self.request_helper.PUT(self.base_url + PATH, self.token, data)
            self.console_log(1,result)
        except:
            self.console_log(2, "Account Update request is failed while sent")

    # Admin
    def user_gets(self):
        try:
            self.console_log(1, "Account Gets request is sent")
            PATH = "/api/User/Users"
            result = self.request_helper.GET(self.base_url + PATH,self.token)
            self.console_log(1,result)
            return result
        except:
            self.console_log(2, "Account Gets request is failed while sent")
    # Admin
    def user_delete(self,email):
        try:
            self.console_log(1, "Account delete request is sent")
            PATH = "/api/User?Email=" + email
            result = self.request_helper.DELETE(self.base_url + PATH, self.token,None)
            self.console_log(1,result)
        except Exception as ex:
            print (ex)
            self.console_log(2, "Account delete request is failed while sent")

    # PROFILE
    def profile_get(self,email):
        try:
            self.console_log(1, "Profile Get request is sent")
            PATH = "/api/Profile?Email=" + email
            result = self.request_helper.GET(self.base_url + PATH,self.token)
            self.console_log(1,result)
        except:
            self.console_log(2, "Profile Get request is failed while sent")
            
    def profile_create(self,hobby,address,birthday):
        try:
            self.console_log(1, "Profile Create request is sent")
            PATH = "/api/Profile"
            data = {"hobby":hobby ,"address":address,"birthday":birthday}
            result = self.request_helper.POST(self.base_url+PATH,self.token,data)
            self.console_log(1,result)
        except:
            self.console_log(2, "Profile Create  request is failed while sent")

    def profile_update(self,hobby,address,birthday):
        try:
            self.console_log(1, "Profile Update request is sent")
            PATH = "/api/Profile"
            data = {"hobby":hobby ,"address":address,"birthday":birthday}
            result = self.request_helper.PUT(self.base_url+PATH,self.token,data)
            self.console_log(1,result)
        except:
            self.console_log(2, "Profile Update request is failed while sent")

    def profile_delete(self,email):
        try:
            self.console_log(1, "Profile Delete request is sent")
            PATH = "/api/Profile?Email=" + email
            result = self.request_helper.DELETE(self.base_url + PATH, self.token, None)
            self.console_log(1,result)
        except:
            self.console_log(2, "Profile Delete request is failed while sent")
    
    def profile_showUserProfile(self):
        try:
            self.console_log(1, "Profile ShowUserProfile request is sent")
            PATH = "/api/Profile/ShowUserProfile"
            result = self.request_helper.GET(self.base_url + PATH,self.token)
            self.console_log(1,result)
        except:
            self.console_log(2, "Profile ShowUserProfile request is failed while sent")

    # CARD

    def card_get(self):
        try:
            self.console_log(1, "Card Get request is sent")
            PATH = "/api/Card"
            result = self.request_helper.GET(self.base_url + PATH,self.token)
            self.console_log(1,result)
        except:
            self.console_log(2, "Card Get request is failed while sent")
            
    def card_getv2(self):
        try:
            self.console_log(1, "Card Get v2 request is sent")
            PATH = "/api/Card/GetCardV2"
            result = self.request_helper.GET(self.base_url + PATH,self.token)
            self.console_log(1,result)
            return result
        except:
            self.console_log(2, "Card Get v2 request is failed while sent")
            
    def card_create(self,nickname,number,expireDate,cve,password):
        try:
            self.console_log(1, "Card Create request is sent")
            PATH = "/api/Card"
            data = {"nickname":nickname ,"number":number,"expireDate":expireDate,"cve":cve,"password":password}
            result = self.request_helper.POST(self.base_url+PATH,self.token,data)
            self.console_log(1,result)
        except:
            self.console_log(2, "Card Create  request is failed while sent")

    def card_delete(self,cardId):
        try:
            self.console_log(1, "Card Delete request is sent")
            PATH = "/api/Card"
            data = {"cardId":cardId}
            result = self.request_helper.DELETE(self.base_url + PATH,self.token,data)
            self.console_log(1,result)
        except:
            self.console_log(2, "Card Delete request is failed while sent")

    # HELPER

    def helper_systemdate(self):
        try:
            self.console_log(1, "Helper SystemDate request is sent")
            PATH = "/api/Helper/SystemDate"
            result = self.request_helper.GET(self.base_url + PATH,self.token)
            self.console_log(1,result)
        except:
            self.console_log(2, "Helper SystemDate request is failed while sent")

    def helper_UploadFile(self):
        try:
            self.console_log(1, "Helper UploadFile request is sent")
            PATH = "/api/Helper/UploadFile"
            files = {'file': open('dummy.pdf','rb')}
            result = self.request_helper.File_Upload(self.base_url + PATH ,self.token,files)
            self.console_log(1,result)
        except:
            self.console_log(2, "Helper UploadFile request is failed while sent")
    
    def helper_listFile(self,username):
        try:
            self.console_log(1, "Helper ListFile request is sent")
            PATH = "/api/Helper/ListFile?name=" + username
            result = self.request_helper.GET(self.base_url + PATH,self.token)
            self.console_log(1,result)
            return result
        except:
            self.console_log(2, "Helper ListFile request is failed while sent")

    def helper_ShowLog(self):
        try:
            self.console_log(1, "Helper ShowLog request is sent")
            PATH = "/api/Helper/ShowLog"
            result = self.request_helper.GET(self.base_url + PATH,self.token)
            self.console_log(1,result)
        except:
            self.console_log(2, "Helper ShowLog request is failed while sent")


    def helper_GetImageFromRemote(self,url):
        try:
            self.console_log(1, "Helper GetImageFromRemote request is sent")
            PATH = "/api/Helper/GetImageFromRemote?url=" + url
            result = self.request_helper.GET(self.base_url + PATH,self.token)
            self.console_log(1,result)
        except:
            self.console_log(2, "Helper GetImageFromRemote request is failed while sent")

    def helper_GetImageFromLocal(self,filename):
        try:
            self.console_log(1, "Helper GetImageFromLocal request is sent")
            PATH = "/api/Helper/GetImageFromLocal?filename=" + filename
            result = self.request_helper.GET(self.base_url + PATH, self.token)
            #self.console_log(1,result)
        except:
            self.console_log(2, "Helper GetImageFromLocal request is failed while sent")

    def console_log(self,status,log):
        if status == 1:
            print ("[*] " + log)
        else:
            print ("[!] " + log)

def user_main(BASE_URL):

    r = RandomDataHelper()
    name,surname,password,email = r.userGenerate()
    hobby,address,date = r.profileGenerate()
    card_name,card_number,card_date,card_cve,card_pass = r.cardGenerate()

    updated_name = r.string()
    updated_surname = r.string()
    updated_password = r.string()
    updated_hobby = r.string()
    updated_address = r.string()

    v = VulnerableApp4API(BASE_URL)
    
    # ACCOUNT
    v.account_register(name,surname,email,password) # ok
    v.account_temporaray_login(email,password) #ok
    v.account_login(email,password) # ok
    v.account_tokenstatus() #ok
    v.account_refreshtoken()

    
    # USER
    v.user_get(email) # ok
    v.user_update(email,updated_name,updated_surname,updated_password) # ok

    # PROFILE
    v.profile_delete(email)
    v.profile_create(hobby,address,date) # ok
    v.profile_get(email) # ok
    v.profile_showUserProfile()
    v.profile_update(updated_hobby,updated_address,date) # ok
    v.profile_get(email) 
    v.profile_delete(email) # ok
    v.profile_create(hobby,address,date)
    v.profile_get(email)
    v.profile_showUserProfile()

    # CARD
    v.card_get() 
    v.card_getv2() # ok
    v.card_create(card_name,card_number,card_date,card_cve,card_pass) # ok
    v.card_get()
    
    temp_card_id = v.card_getv2()
    card_name,card_number,card_date,card_cve,card_pass = r.cardGenerate()
    v.card_create(card_name,card_number,card_date,card_cve,card_pass) # ok
    v.card_getv2()
    v.card_delete(json.loads(temp_card_id)[0]["id"])
    v.card_getv2()

    # HELPER
    v.helper_systemdate() # ok
    v.helper_UploadFile()
    result = v.helper_listFile(updated_name)
    v.helper_GetImageFromLocal(result.split(",")[0].split("name ")[1])
    v.helper_GetImageFromRemote("https://images.unsplash.com/photo-1615796153287-98eacf0abb13")
    v.helper_UploadFile()
    v.helper_listFile(updated_name) 
    

def admin_main(BASE_URL):

    r = RandomDataHelper()

    email = "erdem@star.com"
    password = "erdem"

    hobby,address,date = r.profileGenerate()

    card_name,card_number,card_date,card_cve,card_pass = r.cardGenerate()

    updated_hobby = r.string()
    updated_address = r.string()


    v = VulnerableApp4API(BASE_URL)
    
    # Account
    v.account_login(email,password)
    result = v.user_gets() # ok

    if len(json.loads(result)) % 2 == 0:
        v.user_delete(json.loads(result)[1]["email"])

    v.user_gets()
    
    # PROFILE
    v.profile_delete(email)
    v.profile_create(hobby,address,date) # ok
    v.profile_get(email) # ok
    v.profile_update(updated_hobby,updated_address,date) # ok
    v.profile_get(email) 
    v.profile_delete(email) # ok
    v.profile_create(hobby,address,date)
    v.profile_get(email)

    # CARD

    v.card_get()
    v.card_getv2()
    v.card_create(card_name,card_number,card_date,card_cve,card_pass)
    v.card_get()
    temp_card_id = v.card_getv2()
    
    card_name,card_number,card_date,card_cve,card_pass = r.cardGenerate()
    v.card_create(card_name,card_number,card_date,card_cve,card_pass)

    v.card_getv2()
    v.card_delete(json.loads(temp_card_id)[0]["id"])
    v.card_getv2()

    # HELPER

    v.helper_systemdate()
    v.helper_UploadFile()
    result = v.helper_listFile("Erdem")
    v.helper_GetImageFromLocal(result.split(",")[0].split("name ")[1])
    v.helper_GetImageFromRemote("https://images.unsplash.com/photo-1615796153287-98eacf0abb13")
    v.helper_UploadFile()
    v.helper_listFile("Erdem")
    v.helper_ShowLog()

if len(sys.argv) == 3:
    url = sys.argv[1]
    counter = sys.argv[2]

    for i in range (int(counter)):
        user_main(url)
        if i % 5 == 0:
            admin_main(url)
else:
    print ("[!] python dummy_data.py http://localhost:12134 10000")
