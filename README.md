# TPL_DataFlow

class DataBefore {}
class DataAfter {}

int numberOfThreads = 10;

var dBefore = List<<DataBefore>>();
var dAfter = new List<<DataAfter>>();

var pWorker = new ParallelWorkerWrapper<DataBefore, DataAfter>(numberOfThreads);
dAfter = pWorker.Run(dBefore);
