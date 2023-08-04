import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlumnoInfoDto } from '@pages/expediente-alumno/expediente-models';
import { ExpedienteService } from '@pages/expediente-alumno/expediente.service';
import { CultureService } from '@src/app/services/culture.service';
import { Guid } from 'guid-typescript';
import { BlockUIService } from 'ng-block-ui';
import { Subject } from 'rxjs';
import { AlumnoService } from '../alumno.service';
import { takeUntil } from 'rxjs/operators';
@Component({
  selector: 'app-show',
  templateUrl: './show.component.html',
  styleUrls: ['./show.component.scss']
})
export class ShowComponent implements OnInit, OnDestroy {
  private unsubscribe$ = new Subject<boolean>();
  idIntegracionAlumno: number;
  alumno: AlumnoInfoDto;
  public blockIdentity = Guid.create().toString();

  constructor(
    private route: Router,
    private blockUI: BlockUIService,
    private activatedRoute: ActivatedRoute,
    public alumnoService: AlumnoService,
    private expedienteService: ExpedienteService,
    private cultureService: CultureService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(({ id }) => {
      if (id === undefined) {
        this.route.navigateByUrl('/');
        return;
      }
      this.idIntegracionAlumno = id;
      this.GetAlumno();
    });
  }

  GetAlumno(): void {
    this.blockUI.start(this.blockIdentity);
    this.expedienteService
      .getAlumno(this.idIntegracionAlumno)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((response) => {
        this.alumno = response;
        this.alumnoService.alumnoInfoDto = this.alumno;
        this.blockUI.stop(this.blockIdentity);
      });
  }

  ngOnDestroy(): void {
    this.cultureService.setCulture$ = null;
    this.unsubscribe$.next(true);
    this.unsubscribe$.complete();
  }
}
