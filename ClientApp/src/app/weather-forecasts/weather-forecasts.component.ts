import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-weather-forecasts',
  templateUrl: './weather-forecasts.component.html',
  styleUrls: ['./weather-forecasts.component.css']
})
export class WeatherForecastsComponent implements OnInit {
  public forecasts?: WeatherForecast[];

  constructor(http: HttpClient) {
    http.get<WeatherForecast[]>('/api/weatherforecast').subscribe(result => {
      this.forecasts = result;
    }, error => console.error(error));
  }


  ngOnInit(): void {
  }


}
interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}