import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {

  items: MenuItem[] = [];

  activeItem: MenuItem = {};

  ngOnInit() {
      this.items = [
          {label: 'Denali', disabled: true, styleClass: 'title-text'},
          {label: 'Alerts', icon: 'pi pi-fw pi-bell'},
          {label: 'Portfolio', icon: 'pi pi-fw pi-chart-line'},
          {label: 'Settings', icon: 'pi pi-fw pi-cog'},
          {label: 'Connected', icon: 'pi pi-fw pi-times-circle', styleClass: 'connected-item'}
      ];

      this.activeItem = this.items[1];
  }
}
