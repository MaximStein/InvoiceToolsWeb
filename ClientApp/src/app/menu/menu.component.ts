import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../shared/services/auth.service';


@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {

  public isUserAuthenticated: boolean;

  constructor( protected authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.authService.authChanged
      .subscribe(res => {
        this.isUserAuthenticated = res;
      })
  }

  logout() {
    this.authService.logout();
    this.router.navigate(["/"]);
  }

}
