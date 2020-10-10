
export function getView<TView>(url: string, validatorFunc?: (view: TView) => boolean) : Promise<TView> {

  let attempts = 0;
  const pollStatus = async (resolve: any, reject: any) => {
    attempts++;
    console.log(`Polling view ${url}. Attempt ${attempts}`);
    const res = await fetch(url);
    if (res.status == 200) {
      const view = await res.json() as TView;
      if (!validatorFunc || validatorFunc(view)) {
        resolve(view);
        return;
      }
    }
    if (attempts >= 20) {
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