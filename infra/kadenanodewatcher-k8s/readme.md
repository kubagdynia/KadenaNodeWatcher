This is a Kubernetes deployment for the Kadena Node Watcher.

```bash
kubectl apply -f k8s-deployments.yaml
kubectl get all 
kubectl delete -f k8s-deployments.yaml 

kubectl describe pod webapi-deployment-85cbc7cdf8-t4bx
kubectl logs nginx-deployment-9dfff4c95-lsm57 

```

install Prometheus
```bash
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update
helm install prometheus prometheus-community/prometheus
```

install Grafana
```bash
helm repo add grafana https://grafana.github.io/helm-charts
helm repo update
helm install grafana grafana/grafana
```