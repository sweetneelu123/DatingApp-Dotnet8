import { NgFor } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,NgFor],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {

//(here we are not using constructor to implement any service , we are using it directly to creating instance of AppComponent)
 appHttp = inject(HttpClient);
  title = 'DatingApp';
  users : any;

  ngOnInit(): void {
   this.appHttp.get("https://localhost:5001/API/users").subscribe({
    next: response => this.users = response,
    error: error => console.log(error),
    complete: () => console.log('Request has completed')

   })                   
  }
}
