// noinspection JSUnusedGlobalSymbols
/**
 * @param {DotNetObjectReference} ref
 * @returns {Promise}
 */
export async function viewTransition(ref) {
    if (!document.startViewTransition) {
        await ref.invokeMethodAsync("call");
        return;
    }

    let vt = document.startViewTransition(async () => await ref.invokeMethodAsync("call"))

    await vt.finished;
}

/**
 * @typedef DotNetObjectReference
 * @property {(methodName: string, ...args: any[]) => Promise<any>} invokeMethodAsync
 */