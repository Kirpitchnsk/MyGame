using System.Collections.Generic;

namespace SibGameJam2026.MergeService {
	public interface IMergeSystem {
		bool TryGetMergedProductId(IReadOnlyList<int> inputProductIds, out int outputProductId);

		bool TryGetSourceProductIds(int outputProductId, out IReadOnlyList<int> sourceProductIds);
	}
}
