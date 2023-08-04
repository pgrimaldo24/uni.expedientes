import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DownloadFileService {
  constructor() {}

  downloadFile(data: BlobPart, type: string, filename: string): void {
    const blob = new Blob([data], { type: type });
    const url = window.URL.createObjectURL(blob);

    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);

    a.href = url;
    a.download = filename;
    a.click();
  }
}
