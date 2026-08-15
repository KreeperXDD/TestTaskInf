import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  
  private readonly http = inject(HttpClient);

  selectedFile: File | null = null;
  message = signal('');
  isLoading = signal(false);

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    this.selectedFile = input.files?.[0] ?? null;
    this.message.set('');
  }

  uploadFile(): void {
    if (!this.selectedFile) {
      this.message.set ('Сначала выбери CSV-файл.');
      return;
    }

    const formData = new FormData();
    formData.append('file', this.selectedFile);

    this.isLoading.set(true);
    this.message.set('');

    this.http
      .post('http://localhost:5029/api/files/upload', formData)
      .subscribe({
        next: () => {
          this.message.set(`Файл «${this.selectedFile?.name}» успешно обработан.`);
          this.isLoading.set(false);
        },
        error: () => {
          this.message.set('Не удалось загрузить файл. Проверь, что API запущен.');
          this.isLoading.set(false);
        },
      });
  }
}