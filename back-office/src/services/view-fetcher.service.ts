import { Observable, throwError } from 'rxjs';
import { ajax } from 'rxjs/ajax';
import { delay, filter, repeatWhen, take, timeoutWith } from 'rxjs/operators';

interface GetViewOptions<TView> {
  revision?: number;
  token?: string;
  validatorFunc?: (view: TView) => boolean;
}

interface View {
  revision: number;
}

export function getView$<TView extends View>(url: string, options?: GetViewOptions<TView>): Observable<TView> {
  return ajax
    .getJSON<TView>(url, { Authorization: `Bearer ${options?.token}` })
    .pipe(
      repeatWhen((obs) => obs.pipe(delay(1000))),
      filter((view) => view.revision >= (options?.revision || 0)),
      filter((view) => !options?.validatorFunc || options.validatorFunc(view)),
      take(1),
      timeoutWith(
        10000,
        throwError({
          code: 'Timeout',
          message: 'View timed out',
        })
      )
    );
}
