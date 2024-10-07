This is a Kubernetes deployment for the Kadena Node Watcher.

```bash
kubectl apply -f k8s-deployments.yaml
kubectl get all 
kubectl delete -f k8s-deployments.yaml 

kubectl describe pod webapi-deployment-85cbc7cdf8-t4bx
kubectl logs nginx-deployment-9dfff4c95-lsm57 

```