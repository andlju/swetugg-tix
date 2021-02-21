
interface GetViewOptions<TView> {
  revision?: number,
  token?: string,
  validatorFunc?: (view: TView) => boolean;
}

interface View {
  revision: number,
}

export function getView<TView extends View>(url: string, options?: GetViewOptions<TView>): Promise<TView> {

  let attempts = 0;
  const validator = options && (options.validatorFunc ?? ((view: TView) => !(options.revision) || view.revision >= options.revision));

  const pollStatus = async (resolve: ((view: TView) => void), reject: ((err: { code: string, message: string; }) => void)) => {
    attempts++;
    console.log(`Polling view ${url}. Attempt ${attempts}`);
    const res = await fetch(url, {
      headers: {
        'Authorization': `Bearer ${options?.token}`,
      }
    });
    if (res.status == 200) {
      const view = await res.json() as TView;
      if (!validator || validator(view)) {
        resolve(view);
        return;
      }
    }
    if (attempts >= 10) {
      reject({
        code: 'Timeout',
        message: 'View timed out'
      });
      return;
    }
    setTimeout(() => pollStatus(resolve, reject), 1000);
  };

  const promise = new Promise<TView>((resolve, reject) => {
    pollStatus(resolve, reject);
  });
  return promise;
}