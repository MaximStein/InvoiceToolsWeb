import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Subject } from 'rxjs';
import { AuthResponseDto } from 'src/types/response/authResponseDto';
import { UserLoginDto } from 'src/types/user/userLoginDto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private authChangeSub = new Subject<boolean>()
  public authChanged = this.authChangeSub.asObservable();

  constructor( private http: HttpClient, private jwtHelper: JwtHelperService) { }

 // public sendAuthStateChangeNotification = (isAuthenticated: boolean) => {
 //   this.authChangeSub.next(isAuthenticated);
 // }

  public register(formData:any){
    return this.http.post('api/user/create', formData)
  }

  public login (formData:UserLoginDto) {
    return this.http.post<AuthResponseDto>('api/user/login', formData);    
    
  }

  public isUserAuthenticated():boolean  {
    const token = localStorage.getItem("token");
    
    return token != null && !this.jwtHelper.isTokenExpired(token!);
  }

  public isUserAdmin():boolean  {
    const token = localStorage.getItem("token");
    const decodedToken = this.jwtHelper.decodeToken(token!);
    const role = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
    return role === 'Administrator';
  }

  public logout() {
    console.log("logging out ");
    localStorage.removeItem("token");
    //this.sendAuthStateChangeNotification(false);
  }

}
