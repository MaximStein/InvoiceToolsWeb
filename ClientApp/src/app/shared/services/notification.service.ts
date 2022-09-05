import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(private _snackBar:MatSnackBar) { }

  static defaultDuration = 5000;

  showNotification(text:string, duration=NotificationService.defaultDuration) {
    var sb = this._snackBar.open(text, "ok", { duration: duration });
  }

  showGenericError(duration = NotificationService.defaultDuration) {
    this.showNotification("Es ist ein Fehler aufgetreten");
  }

  showGenericSuccess(duration = NotificationService.defaultDuration) {
    this.showNotification("Die Aktion wurde erfolgreich abgeschlossen");
  }
}
