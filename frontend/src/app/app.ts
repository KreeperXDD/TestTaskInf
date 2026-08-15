import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

interface ResultItem {
  fileName: string;
  firstOperationDate: string;
  timeDeltaSeconds: number;
  avarageMetricValue: number;
  medianMetricValue: number;
  minMetricValue: number;
  maxMetricValue: number;
  avarageExecutionTime: number;
}

interface ValueItem {
  fileName: string;
  date: string;
  metricValue: number;
  executionTime: number;
}

@Component({
  selector: 'app-root',
  imports: [DatePipe, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  private readonly http = inject(HttpClient);

  selectedFile: File | null = null;
  message = signal('');
  isLoading = signal(false);

  results = signal<ResultItem[]>([]);
  resultsMessage = signal('');
  isResultsLoading = signal(false);

  latestValues = signal<ValueItem[]>([]);
  selectedResultFile = signal('');
  valuesMessage = signal('');
  isValuesLoading = signal(false);

  fileNameFilter = '';
  firstOperationDateFrom = '';
  firstOperationDateTo = '';
  avarageMetricValueFrom = '';
  avarageMetricValueTo = '';
  avarageExecutionTimeFrom = '';
  avarageExecutionTimeTo = '';

  ngOnInit(): void {
    this.loadResults();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    this.selectedFile = input.files?.[0] ?? null;
    this.message.set('');
  }

  uploadFile(): void {
    if (!this.selectedFile) {
      this.message.set('Сначала выбери CSV-файл.');
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
          this.message.set(
            `Файл «${this.selectedFile?.name}» успешно обработан.`
          );
          this.isLoading.set(false);
          this.loadResults();
        },
        error: () => {
          this.message.set(
            'Не удалось загрузить файл. Проверь, что API запущен.'
          );
          this.isLoading.set(false);
        },
      });
  }

  loadResults(): void {
    let params = new HttpParams();

    if (this.fileNameFilter) {
      params = params.set('fileName', this.fileNameFilter);
    }

    if (this.firstOperationDateFrom) {
      params = params.set(
        'firstOperationDateFrom',
        this.firstOperationDateFrom
      );
    }

    if (this.firstOperationDateTo) {
      params = params.set('firstOperationDateTo', this.firstOperationDateTo);
    }

    if (this.avarageMetricValueFrom) {
      params = params.set(
        'avarageMetricValueFrom',
        this.avarageMetricValueFrom
      );
    }

    if (this.avarageMetricValueTo) {
      params = params.set(
        'avarageMetricValueTo',
        this.avarageMetricValueTo
      );
    }

    if (this.avarageExecutionTimeFrom) {
      params = params.set(
        'avarageExecutionTimeFrom',
        this.avarageExecutionTimeFrom
      );
    }

    if (this.avarageExecutionTimeTo) {
      params = params.set(
        'avarageExecutionTimeTo',
        this.avarageExecutionTimeTo
      );
    }

    this.isResultsLoading.set(true);
    this.resultsMessage.set('');

    this.http
      .get<ResultItem[]>('http://localhost:5029/api/results', { params })
      .subscribe({
        next: (data) => {
          this.results.set(data);
          this.isResultsLoading.set(false);
        },
        error: () => {
          this.resultsMessage.set(
            'Не удалось получить результаты. Проверь введённые фильтры.'
          );
          this.isResultsLoading.set(false);
        },
      });
  }

  clearFilters(): void {
    this.fileNameFilter = '';
    this.firstOperationDateFrom = '';
    this.firstOperationDateTo = '';
    this.avarageMetricValueFrom = '';
    this.avarageMetricValueTo = '';
    this.avarageExecutionTimeFrom = '';
    this.avarageExecutionTimeTo = '';

    this.loadResults();
  }

  loadLatestValues(fileName: string): void {
    const params = new HttpParams().set('fileName', fileName);

    this.selectedResultFile.set(fileName);
    this.latestValues.set([]);
    this.valuesMessage.set('');
    this.isValuesLoading.set(true);

    this.http
      .get<ValueItem[]>('http://localhost:5029/api/values/latest', { params })
      .subscribe({
        next: (data) => {
          this.latestValues.set(data);
          this.isValuesLoading.set(false);
        },
        error: () => {
          this.valuesMessage.set(
            'Не удалось получить операции выбранного файла.'
          );
          this.isValuesLoading.set(false);
        },
      });
  }
}