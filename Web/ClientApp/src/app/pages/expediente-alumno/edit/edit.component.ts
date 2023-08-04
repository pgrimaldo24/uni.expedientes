import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class EditComponent implements OnInit {
  idAlumno: number;
  section: string;
  constructor(
    private activatedRoute: ActivatedRoute,
    private route: Router,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.section = (this.location.getState() as { section: string })?.section;
    if (!this.section) this.section = 'expediente';
    this.activatedRoute.params.subscribe(({ id }) => {
      if (id === undefined) {
        this.route.navigateByUrl('/');
      }
      this.idAlumno = id;
    });
  }
}
